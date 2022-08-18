namespace EnkuMessenger;

using System;

/// <summary>
/// メッセンジャーパターンを提供するクラス用のインターフェース
/// </summary>
/// <typeparam name="T">メッセージの型</typeparam>
public interface IMessenger<T> where T : class
{
    /// <summary>
    /// 受信側管理リスト内の要素の参照が切れているか確認し
    /// 切れているのならコレクションから除外するメソッド
    /// </summary>
    void CheckAlive();

    /// <summary>
    /// メッセージ受信側の登録用メソッド
    /// </summary>
    /// <param name="receiver">受信側のオブジェクト</param>
    /// <param name="filtter">フィルターメソッド</param>
    void Register(IReceiver<T> receiver, Func<T, bool>? filtter = null);

    /// <summary>
    /// すでに受信側が登録されてるかチェックするためのメソッド
    /// </summary>
    /// <param name="receiver"></param>
    /// <returns>
    /// すでに登録済みならばtrueを返す
    /// 登録されていても参照が切れているのなら
    /// 受信側管理リストから削除してfalseを返す
    /// </returns>
    bool IsRegistered(IReceiver<T> receiver);

    /// <summary>
    /// IReceiverオブジェクトに対応する受信側クラスの登録解除用メソッド
    /// </summary>
    /// <param name="receiver">削除対象のオブジェクト</param>
    void UnRegister(IReceiver<T> receiver);

    /// <summary>
    /// 全ての登録済み受信側クラスの削除用メソッド
    /// </summary>
    void UnRegisterAll();

    /// <summary>
    /// メッセージ送信用メソッド
    /// </summary>
    /// <param name="parameter">メッセージパラメータ</param>
    void Send(T parameter);

    /// <summary>
    /// 登録済みIReciverの数をカウント
    /// </summary>
    /// <returns>登録済みIReciverの数</returns>
    int RegisteredCount();
}
