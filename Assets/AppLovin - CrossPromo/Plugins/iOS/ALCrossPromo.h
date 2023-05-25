//
//  ALCrossPromo.h
//  AppLovin MAX Cross Promo Kit for Lion Studios
//
//  Created by Thomas So on 3/21/19.
//  Copyright © 2019 AppLovin Corporation. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <AppLovinSDK/AppLovinSDK.h>

/**
 * This is a library for AppLovin Cross Promo integration. It is designed to work from the Unity app context.
 */
@interface ALCrossPromo : NSObject<ALAdLoadDelegate, ALAdDisplayDelegate, ALAdViewEventDelegate>

/**
 * Show the cross promo MRec with the given parameters.
 *
 * @param xOffsetPercent The top-left x position, as a percentage of the screen width.
 * @param yOffsetPercent The top-left y position, as a percentage of the screen height.
 * @param widthPercent   The width, as a percentage of the smaller screen dimension.
 * @param heightPercent  The height, as a percentage of the smaller screen dimension.
 * @param rotation       The clock-wise rotation, in degrees.
 */
- (void)showMRec:(float)xOffsetPercent
               y:(float)yOffsetPercent
           width:(float)widthPercent
          height:(float)heightPercent
        rotation:(float)rotation;

/**
 * Hides the current MRec from the screen.
 */
- (void)hideMRec;

@end
