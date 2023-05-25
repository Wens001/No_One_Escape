
/****************************************************
 * FileName:		XCodeProjectProcess.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-07-31-17:45:13
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
//using UnityEditor.iOS.Xcode;
using UnityEngine;

public class XCodeProjectProcess
{
#if UNITY_IOS
    [PostProcessBuild]
	public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
    {
        Debug.Log("构建完成，" + path);

        if (buildTarget == BuildTarget.iOS)
        {
            //info.plist
            var plistPath = Path.Combine(path, "Info.plist");
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);
            
            plist.root.SetString("NSCalendarsUsageDescription", "Store calendar events from ads");
            plist.root.SetString("NSLocationWhenInUseUsageDescription", "Used to deliver better advertising experience");
            
            
            var exitsOnSuspendKey = "UIApplicationExitsOnSuspend";
            if(plist.root.values.ContainsKey(exitsOnSuspendKey))
            {
                plist.root.values.Remove(exitsOnSuspendKey);
            }
            

            plist.WriteToFile(plistPath);
            
            //project
            var projPath = PBXProject.GetPBXProjectPath(path);
            var proj = new PBXProject();
            proj.ReadFromString(File.ReadAllText(projPath));
            //var target = proj.TargetGuidByName(PBXProject.GetUnityTargetName());
            var target = proj.GetUnityMainTargetGuid();

            //add framework
            proj.AddFrameworkToProject(target, "AdSupport.framework", false);
            proj.AddFrameworkToProject(target, "CoreTelephony.framework", false);
            proj.AddFrameworkToProject(target, "StoreKit.framework", false);
            proj.AddFrameworkToProject(target, "WebKit.framework", false);
            
            proj.AddFrameworkToProject(target, "iAd.framework", false);

            proj.AddFileToBuild(target, proj.AddFile("usr/lib/libz.tbd", "Frameworks/libz.tbd", PBXSourceTree.Sdk));

            //property
            proj.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
            proj.AddBuildProperty(target, "OTHER_LDFLAGS", "-ObjC");

            File.WriteAllText(projPath, proj.WriteToString());
        }
    }
#endif
}
