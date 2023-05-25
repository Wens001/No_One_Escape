//
//  ALCrossPromo.mm
//  AppLovin MAX Cross Promo Kit for Lion Studios
//
//  Created by Thomas So on 3/21/19.
//  Copyright © 2019 AppLovin Corporation. All rights reserved.
//

#import <AppLovinSDK/AppLovinSDK.h>
#import "ALCrossPromo.h"

#define DEGREES_TO_RADIANS(angle) ((angle) / 180.0 * M_PI)

@interface ALCrossPromo()

@property (nonatomic, strong) ALSdk *sdk;

@property (nonatomic, assign) BOOL isCrossPromoViewVisible;
@property (nonatomic, strong) ALAdView *crossPromoMRec;

/**
 * Dynamically retrieve the main view depending on if we're in the AppLovin test app or Unity environment.
 */
@property (nonatomic, weak, readonly) UIView *screenView;

@end

@implementation ALCrossPromo
@dynamic screenView;

static NSString *const kALCrossPromoSDKKey = @"HSrCHRtOan6wp2kwOIGJC1RDtuSrF2mWVbio2aBcMHX9KF3iTJ1lLSzCKP1ZSo5yNolPNw1kCTtWpxELFF4ah1";
static NSString *const kALCrossPromoZoneID = @"4dd4576c0ebefb66";
static NSString *const TAG = @"ALCrossPromo";

extern "C"
{
    #if !IS_APPLOVIN_TEST_APP
    extern UIView* UnityGetGLView();
    #endif
}

#pragma mark - Initialization

- (instancetype)init
{
    self = [super init];
    if ( self )
    {
        self.sdk = [ALSdk sharedWithKey: kALCrossPromoSDKKey];
        [self.sdk setPluginVersion: @"2.0.0"];
        
        [self createAndAttachAdView];
        
        [self loadNextCrossPromoMRec];
    }
    return self;
}

- (void)dealloc
{
    if ( self.crossPromoMRec )
    {
        [self.crossPromoMRec removeFromSuperview];
        self.crossPromoMRec = nil;
    }
}

#pragma mark - MRec Methods

- (void)loadNextCrossPromoMRec
{
    [self.sdk.adService loadNextAdForZoneIdentifier: kALCrossPromoZoneID andNotify: self];
}

- (void)showMRec:(float)xOffsetPercent
               y:(float)yOffsetPercent
           width:(float)widthPercent
          height:(float)heightPercent
        rotation:(float)rotation
{
    if ( !self.crossPromoMRec ) return;
    
    self.isCrossPromoViewVisible = YES;
    self.crossPromoMRec.hidden = NO;
    
    [self updateAdPosition: xOffsetPercent
                         y: yOffsetPercent
                     width: widthPercent
                    height: heightPercent
                  rotation: rotation];
}

- (void)hideMRec
{
    if ( !self.crossPromoMRec ) return;
    
    self.isCrossPromoViewVisible = NO;
    self.crossPromoMRec.hidden = YES;
}

#pragma mark - Ad Load Delegate

- (void)adService:(ALAdService *)adService didLoadAd:(ALAd *)ad
{
    // Paranoia check
    if ( [kALCrossPromoZoneID isEqualToString: ad.zoneIdentifier] )
    {
        [self log: @"Showing next cross-promo MREC ad"];
        [self.crossPromoMRec render: ad];
    }
    else
    {
        [self log: @"CROSS PROMO ERROR: Requested and received Zone IDs do not match. Requested Zone ID: %@; Received Zone ID: %@", kALCrossPromoZoneID, ad.zoneIdentifier];
    }
}

- (void)adService:(ALAdService *)adService didFailToLoadAdWithError:(int)code
{
    [self log: @"FAILED TO GET APPLOVIN AD. ERROR: %d", code];
    
    if ( code != kALErrorCodeNoFill )
    {
        [self performSelector: @selector(loadNextCrossPromoMRec)
                   withObject: nil
                   afterDelay: 10];
    }
}

#pragma mark - Ad Display Delegate

- (void)ad:(ALAd *)ad wasDisplayedIn:(UIView *)view
{
    [self log: @"Cross Promo Ad Displayed."];
    if ( !self.isCrossPromoViewVisible ) self.crossPromoMRec.hidden = YES;
}

- (void)ad:(ALAd *)ad wasHiddenIn:(UIView *)view
{
    [self log: @"Cross Promo Ad Hidden."];
}

- (void)ad:(ALAd *)ad wasClickedIn:(UIView *)view
{
    [self log: @"Cross Promo Ad Clicked."];
}

#pragma mark - Ad View Event Delegate

- (void)ad:(ALAd *)ad didPresentFullscreenForAdView:(ALAdView *)adView
{
    [self log: @"Cross Promo Ad Did Present Full Screen."];
}

- (void)ad:(ALAd *)ad willLeaveApplicationForAdView:(ALAdView *)adView
{
    [self log: @"Cross Promo Ad Clicked. Will Leave Application. Loading Next Ad."];
    [self loadNextCrossPromoMRec];
}

#pragma mark - Helper methods

- (void)updateAdPosition:(float)xOffsetPercent
                       y:(float)yOffsetPercent
                   width:(float)widthPercent
                  height:(float)heightPercent
                rotation:(float)rotation
{
    // Assuming rotations is in degrees
    self.crossPromoMRec.transform = CGAffineTransformRotate(CGAffineTransformIdentity, DEGREES_TO_RADIANS(rotation));
    
    CGRect screenRect = [[UIScreen mainScreen] bounds];
    CGFloat screenWidth = screenRect.size.width;
    CGFloat screenHeight = screenRect.size.height;
    CGFloat minDimensionLength = MIN(screenWidth, screenHeight);
    
    CGFloat x = (xOffsetPercent / 100.0) * screenWidth;
    CGFloat y = (yOffsetPercent / 100.0) * screenHeight;
    CGFloat width = (widthPercent / 100.0) * minDimensionLength;
    CGFloat height = (heightPercent / 100.0) * minDimensionLength;
    
    if ( [self isIPhoneX] )
    {
        if ( @available(iOS 11.0, *) ) // To silence warning
        {
            // PLEASE NOTE: we need to compensate for iPhone X status bar and safe area inset
            y += [UIApplication sharedApplication].keyWindow.safeAreaInsets.top + [UIApplication sharedApplication].statusBarFrame.size.height;
        }
    }
    
    // Update frame
    self.crossPromoMRec.frame = CGRectMake(x, y, width, height);
    
    // We need to update bounds with bigger (if rotated) containing frame to prevent clipping
    self.crossPromoMRec.bounds = CGRectMake(0, 0, width, height); // bounds does not care about x or y
}

- (void)createAndAttachAdView
{
    // Setup cross promo ad view
    self.crossPromoMRec = [[ALAdView alloc] initWithSdk: self.sdk size: [ALAdSize sizeMRec]];
    self.crossPromoMRec.adDisplayDelegate = self;
    self.crossPromoMRec.adEventDelegate = self;
    self.crossPromoMRec.hidden = YES;
    
    [self.screenView addSubview: self.crossPromoMRec];
}

- (void)log:(NSString *)format, ...
{
    va_list valist;
    va_start(valist, format);
    NSString *message = [[NSString alloc] initWithFormat: format arguments: valist];
    va_end(valist);
    
    NSLog(@"[%@] %@", TAG, message);
}

- (UIView *)screenView
{
#if !IS_APPLOVIN_TEST_APP
    return UnityGetGLView();
#else
    return UIApplication.sharedApplication.keyWindow.rootViewController.view;
#endif
}

- (BOOL)isIPhoneX
{
    int maxSizeDimension = (int) MAX(CGRectGetHeight([UIScreen mainScreen].bounds), CGRectGetWidth([UIScreen mainScreen].bounds));
    return (maxSizeDimension == 812) || (maxSizeDimension == 896.0);
}

@end

extern "C"
{
    void _crossPromoCreateInstance();
    void _crossPromoMRecShow(float x, float y, float width, float height, float rotation);
    void _crossPromoMRecHide();
    
    static ALCrossPromo *instance;
    
    void _crossPromoCreateInstance()
    {
        if (instance) return;
        
        instance = [[ALCrossPromo alloc] init];
    }
    
    void _crossPromoShowMRec(float xPercent, float yPercent, float widthPercent, float heightPercent, float rotation)
    {
        if (!instance) return;
        
        [instance showMRec: xPercent
                         y: yPercent
                     width: widthPercent
                    height: heightPercent
                  rotation: rotation];
    }
    
    void _crossPromoHideMRec()
    {
        if (!instance) return;
        
        [instance hideMRec];
    }
}
