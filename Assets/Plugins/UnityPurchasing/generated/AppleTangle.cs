#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class AppleTangle
    {
        private static byte[] data = System.Convert.FromBase64String("WMlnjiIFdLBmhdg8ExIQERCykxAxfncxZXl0MWV5dH8xcGFhfXhycA6UkpQKiCxWJuO4ilGfPcWggQPJIQAXEkQVGwIbUGFhfXQxWH9yPyA9MXJ0Y2V4d3hycGV0MWF+fXhyaHN9dDFiZXB/dXBjdTFldGN8YjFweHd4cnBleH5/MVBkZXl+Y3hlaCCRBTrBeFaFZxjv5XqcP1G35lZcbjFSUCGTEDMhHBcYO5dZl+YcEBAQfXQxWH9yPyA3ITUXEkQVGgIMUGGmCqyCUzUDO9YeDKdcjU9y2VqRBtgIY+RMH8RuTorjNBKrRJ5cTBzgayGTEGchHxcSRAweEBDuFRUSExB2nhmlMebavT0xfmGnLhAhnaZS3mF9dDFDfn5lMVJQIQ8GHCEnISUjNyE1FxJEFRoCDFBhYX10MVJ0Y2U+IZDSFxk6FxAUFBYTEyGQpwuQoq/lYor/w3Ue2mheJcmzL+hp7nrZJ4hdPGmm/J2KzeJmiuNnw2YhXtAOgMoPVkH6FPxPaJU8+iezRl1E/RUXAhNEQiACIQAXEkQVGwIbUGFhnmKQcdcKShg+g6PpVVnhcSmPBORIthQYbQZRRwAPZcKmmjIqVrLEfhcSRAwfFQcVBTrBeFaFZxjv5XqcyCdu0JZEyLaIqCNT6snEYI9vsEPRciJm5isWPUf6yx4wH8urYghepCGTFaohkxKysRITEBMTEBMhHBcYP1G35lZcbhlPIQ4XEkQMMhUJIQduULmJ6MDbd401egDBsqr1CjvSDgchBRcSRBUSAhxQYWF9dDFDfn5lf3Uxcn5/dXhleH5/YjF+dzFkYnRmZj9wYWF9dD9yfnw+cGFhfXRycDXz+sCmYc4eVPA22+B8afz2pAYGIidLIXMgGiEYFxJEFRcCE0RCIAKaCJjP6Fp95Ba6MyET+Qkv6UEYwmV4d3hycGV0MXNoMXB/aDFhcGNlY3ByZXhydDFiZXBldHx0f2ViPyEZOhcQFBQWExAHD3llZWFiKz4+ZlRvDl16QYdQmNVlcxoBklCWIpuQFBESkxAeESGTEBsTkxAQEfWAuBgW/WwokppCMcIp1aCui14beu467TFwf3UxcnRjZXh3eHJwZXh+fzFhpCu85R4fEYMaoDAHP2XELRzKcwe6smCDVkJE0L4+UKLp6vJh3PeyXaAhSf1LFSOdeaKeDM90Yu52T3StJCMgJSEiJ0sGHCIkISMhKCMgJSG5zW8zJNs0xMgex3rFszUyAOawvWF9dDFSdGNleHd4cnBleH5/MVBkkxARFxg7l1mX5nJ1FBAhkOMhOxdleX5jeGVoIAchBRcSRBUSAhxQYR6MLOI6WDkL2e/fpKgfyE8Nx9osaDFwYmJkfHRiMXBycnRhZXB/cnR1JDIEWgRIDKKF5ueNj95Bq9BJQSw3djGbInvmHJPez/qyPuhCe0p1GU8hkxAAFxJEDDEVkxAZIZMQFSFDdH14cH9ydDF+fzFleXhiMXJ0Y4SPax21VppKxQcmItrVHlzfBXjAFyEeFxJEDAIQEO4VFCESEBDuIQwcFxg7l1mX5hwQEBQUERKTEBARTTuXWZfmHBAQFBQRIXMgGiEYFxJEQbubxMv17cEYFiahZGQw");
        private static int[] order = new int[] { 27,41,49,9,44,42,11,32,13,9,59,19,28,21,19,59,37,50,48,59,49,55,52,30,58,57,27,31,50,50,43,36,53,55,37,39,46,39,57,59,51,45,43,58,46,53,54,54,59,58,52,51,58,56,55,57,59,58,58,59,60 };
        private static int key = 17;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
