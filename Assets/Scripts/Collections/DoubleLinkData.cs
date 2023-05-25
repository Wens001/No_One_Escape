
/****************************************************
 * FileName:		DoubleLinkData.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-05-28-15:36:08
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
public class DoubleLinkData<T>
{
    /// <summary>
    /// 连接状态
    /// </summary>
    public bool IsConnect { get { return Next != null; } }

    /// <summary>
    /// 连接对象
    /// </summary>
    private DoubleLinkData<T> Next;
    /// <summary>
    /// 自定义数据
    /// </summary>
    public T Data { get; set; }

    /// <summary>
    /// 终止连接
    /// </summary>
    /// <param name="t"></param>
    public void DisConnect(int t = 1)
    {
        if (!IsConnect)
            return;
        if (t > 0)
            Next.DisConnect(0);
        Next = null;
        Data = default;
    }

    /// <summary>
    /// 连接
    /// </summary>
    /// <param name="link"></param>
    /// <param name="_data"></param>
    /// <param name="t"></param>
    public void Connect(DoubleLinkData<T> link, int t = 1)
    {
        if (link == null || IsConnectMe(link))
            return;
        if (IsConnect)
            DisConnect();
        Next = link;
        Data = link.Data;
        if (t > 0)
            Next.Connect(this, 0);
    }

    /// <summary>
    /// 是否为连接对象
    /// </summary>
    /// <param name="link"></param>
    /// <returns></returns>
    public bool IsConnectMe(DoubleLinkData<T> link)
    {
        if (!IsConnect || link == null)
            return false;
        return link == Next;
    }

}
