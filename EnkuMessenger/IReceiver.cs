namespace EnkuMessenger;

/// <summary>
/// メッセージの受信側で継承して使用するインターフェース
/// </summary>
/// <typeparam name="T">メッセージの型</typeparam>
public interface IReceiver<T>
{
    /// <summary>
    /// メッセージ受信時に実行されるメソッド
    /// </summary>
    /// <param name="message">受信したメッセージ内容</param>
    void ReciveMessage(T message);
}
