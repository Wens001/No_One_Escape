# Cross Promo Kit

This developer kit is designed for Lion publishers to easily integrate cross-promo ads into their Unity applications.

# Integration

## Unity

### Integrate AppLovin SDK

1) Download the latest SDK Unity plugin from https://dash.applovin.com/documentation/mediation/unity/getting-started
2) Import the `AppLovin-MAX-Unity-Plugin-x.x.x-Android-x.x.x-iOS-x.x.x.unitypackage` into your Unity project.

### Integrate Cross-Promo SDK
Import the `applovin-cross-promo.unitypackage` into your Unity project.

### Initialize Cross-Promo SDK

Wherever your game launches, add the following line:

```
AppLovinCrossPromo.Init ();
```

Note: The Cross-Promo SDK handles the AppLovin SDK initialization and ad preloading automatically.

### Display Cross-Promo Button

Use the following example to add a cross-promo button:

```
// Top-left X position expressed as a percentage of the screen width.
const float xPercent = 5.0;

// Top-left Y position expressed as a percentage of the screen height
const float yPercent = 30.0;

// Width expressed as a percentage of the minimum value between screen width and height
const float wPercent = 30.0;

// Height expressed as a percentage of the minimum value between screen width and height
const float hPercent = 30.0;

// Clock-wise rotation expressed in degrees
const float rDegrees = 45.0;

// Show Cross Promo MREC ad
AppLovinCrossPromo.Instance().ShowMRec (xPercent, yPercent, wPercent, hPercent, rDegrees);
```

### Hide Cross-Promo Button

Use the following example to hide the cross-promo button:

```
// Hide currently visible Cross Promo MREC ad
AppLovinCrossPromo.Instance().HideMRec ();
```
