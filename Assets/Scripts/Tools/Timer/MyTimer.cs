//��ʱ��ģ�������
public class MyTimer 
{
    public bool IsFinish { get; protected set; }

    // ��ǰ��ʱ
    public float timer { get; set; }

    //��ǰ����
    public float GetRatioComplete { get { return timer / DurationTime; } }

    //ʣ�����
    public float GetRatioRemaining { get { return 1 - GetRatioComplete; } }

    // ��ʱ��
    public float DurationTime { get; set; }

    // ���¼�ʱ
    public void ReStart()
    {
        timer = 0;
        IsFinish = false;
    }

    // ֡������ʱ
    public void OnUpdate(float deltalTime)
    {
        if (IsFinish)
            return;
        timer += deltalTime;
        if (timer <= 0)
            timer = 0;
        if (timer >= DurationTime)
        {
            timer = DurationTime;
            IsFinish = true;
        }
    }

    public MyTimer(float value = 0.5f )
    {
        DurationTime = value;
    }

    // ��ʱ����
    public void SetFinish()
    {
        IsFinish = true;
    }

}
