namespace EnkuMessenger;

using System;

/// <summary>
/// Messengerで使用する受信側リストの要素の型
/// </summary>
/// <typeparam name="T">メッセージの型</typeparam>
internal class ReceiversElement<T>
{
    /// <summary>
    /// 受信側オブジェクトの弱参照
    /// </summary>
    public WeakReference<IReceiver<T>> Receiver { get; private set; }

    /// <summary>
    /// メッセージ受信時にフィルタリングするための処理が記されたファンクタ
    /// 戻り値はTrueで受信許可
    /// </summary>
    public Func<T, bool>? Filter { get; private set; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ReceiversElement(WeakReference<IReceiver<T>> receiver, Func<T, bool>? filter)
    {
        this.Receiver = receiver;
        this.Filter = filter;
    }
}
