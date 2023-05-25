
/****************************************************
 * FileName:		IHideable.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-05-17-01:39:09
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/

/// <summary>
/// Interface that needs to be implemented by any object that gets affected by the Field of View of the player.
/// </summary>
public interface IHideable {

    void OnFOVEnter();
    void OnFOVLeave();
}
