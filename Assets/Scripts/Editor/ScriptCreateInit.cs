using UnityEngine;
using System.IO;
public class ScriptCreateInit : UnityEditor.AssetModificationProcessor
{
    private const string HeadString = @"
/****************************************************
 * FileName:		#SCRIPTNAME#
 * CompanyName:		#CompanyName#
 * Author:			#Author#
 * Email:			#Email#
 * CreateTime:		#CreateTime#
 * Version:			#Version#
 * UnityVersion:	#UnityVersion#
 * Description:		Nothing
 * 
*****************************************************/
";

    private const string regionStr = @"
    #region --- Public Variable ---


    #endregion


    #region --- Private Variable ---



    #endregion


";


    public static readonly string CompanyName   = ""; 
    public static readonly string Author        = "";
    public static readonly string Email         = "";
    public static readonly string Version       = "1.0";

    public static void OnWillCreateAsset(string newFileMeta)
    {
        string newFilePath = newFileMeta.Replace(".meta", "");
        string fileExt = Path.GetExtension(newFilePath);
        if (fileExt != ".cs")
            return;
        string realPath = Application.dataPath.Replace("Assets", "") + newFilePath;
        string scriptContent = File.ReadAllText(realPath);

        var t = HeadString;
        t = t.Replace("#SCRIPTNAME#", Path.GetFileName(newFilePath));
        t = t.Replace("#CompanyName#", CompanyName);
        t = t.Replace("#Author#", Author);
        t = t.Replace("#Email#", Email);
        t = t.Replace("#Version#", Version);
        t = t.Replace("#UnityVersion#", Application.unityVersion);
        t = t.Replace("#CreateTime#", System.DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss"));



        scriptContent = t + scriptContent;
        var temp = @"   // Start is called before the first frame update";
        if (scriptContent.Contains(temp))
            scriptContent = scriptContent.Replace(temp, regionStr);
        File.WriteAllText(realPath, scriptContent);
    }
}