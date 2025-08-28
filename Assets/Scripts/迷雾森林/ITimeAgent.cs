/// <summary>
/// 能够被角度控制时间流动的对象都实现此接口
/// </summary>
public interface ITimeAgent
{
    /// <param name="timeDelta">时间增量（秒）。可为负，负表示倒退</param>
    void ApplyTime(float timeDelta);
}
