#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("yH8fVKWUq478Xh7stqhSw8uNV8ZKqyeL4oYH0Ek99JnJzAV4VGIy3XLAQ2ByT0RLaMQKxLVPQ0NDR0JB43tIYsvK5FKRY91GDR6cM1zwrSUH3Z9uCfhenklDym6ThbF+2mYtKvie+RB4CnOj62LPzTwYqiYnCmnO0HbHvjoKjUxPPZTa0Ed2YGDzipL7GlDONAzS67B/ket/nJsZSX+tcB8PDxGZeU3H/75B4ztKc0TicwdrmgN5WmyZjxuC5HVFYaJYEy0zPNLAQ01CcsBDSEDAQ0NC4D6t3AT3d6TQxghrIKzu923SFbyGI7igDaQShLfav4DpSwpnZWZ1j7kDF0GVk5O9OSVbtuk79URrpIEaqW6zxtaM47K7PiRoTzvhr0BBQ0JD");
        private static int[] order = new int[] { 7,5,7,4,12,10,11,9,11,13,10,12,12,13,14 };
        private static int key = 66;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
