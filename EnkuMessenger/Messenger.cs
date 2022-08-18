namespace EnkuMessenger;

using System;
using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// メッセンジャーパターンを提供するクラス
/// </summary>
/// <remarks>
/// 使用するViewModelのテスト時にMoc可能なように
/// シングルトンクラスとしてIMessengerインターフェースを継承している。
/// また、prismにはメッセンジャーパターンに相当するEventAggregatorなる機構が存在するが
/// ViewModelでのコンストラクタインジェクションにてのみ取得可能なIEventAggregatorオブジェクトからしか
/// 登録ができないのでメッセンジャーパターンを自作する事とした。
/// </remarks>
/// <typeparam name="T">メッセージの型</typeparam>
public class Messenger<T> : IMessenger<T> 
    where T : class
{
    private static Lazy<Messenger<T>> s_instance = new(() => new Messenger<T>());
    private List<ReceiversElement<T>> _receivers = new();
    private object _lockObject = new();

    // シングルトンクラスなのでコンストラクタは非公開となっている
    private Messenger()
    {
    }

    /// <summary>
    /// インスタンス取得用プロパティ
    /// </summary>
    public static Messenger<T> Instance => s_instance.Value;

    /// <summary>
    /// 受信側管理リスト内の要素の参照が切れているか確認し
    /// 切れているのならコレクションから除外するメソッド
    /// </summary>
    public void CheckAlive()
    {
        lock (this._lockObject)
        {
            foreach (var element in this._receivers)
            {
                // 参照が切れているのなら削除
                if (!element.Receiver.TryGetTarget(out var current))
                    this._receivers.Remove(element);
            }
        }
    }

    /// <summary>
    /// メッセージ受信側の登録用メソッド
    /// </summary>
    /// <param name="receiver">受信側のオブジェクト</param>
    /// <param name="filtter">フィルターメソッド</param>
    public void Register(IReceiver<T> receiver, Func<T, bool>? filtter = null)
    {
        var weakReceiver = new WeakReference<IReceiver<T>>(receiver);
        var element = new ReceiversElement<T>(weakReceiver, filtter);

        if (!this.IsRegistered(receiver))
        {
            lock (this._lockObject)
            {
                this._receivers.Add(element);
            }
        }
    }

    /// <summary>
    /// すでに受信側が登録されてるかチェックするためのメソッド
    /// </summary>
    /// <param name="receiver"></param>
    /// <returns>
    /// すでに登録済みならばtrueを返す
    /// 登録されていても参照が切れているのなら
    /// 受信側管理リストから削除してfalseを返す
    /// </returns>
    public bool IsRegistered(IReceiver<T> receiver)
    {
        var result = false;

        lock (this._lockObject)
        {
            // リスト内の検索
            var index = this._receivers.FindIndex(element => 
            {
                if (element.Receiver.TryGetTarget(out var receiverRef))
                {
                    return object.ReferenceEquals(receiverRef, receiver);
                }
                else
                {
                    return false;
                }
            });

            // 引数に渡せる参照を持っている時点で参照が切れていない根拠となるので参照チェックは行わない
            result = index >= 0;
        }

        return result;
    }

    /// <summary>
    /// IReceiverオブジェクトに対応する受信側クラスの登録解除用メソッド
    /// </summary>
    /// <param name="receiver">削除対象のオブジェクト</param>
    public void UnRegister(IReceiver<T> receiver)
    {
        lock (this._lockObject)
        {
            this._receivers.RemoveAll(element =>
            {
                if (element.Receiver.TryGetTarget(out var receiverRef))
                {
                    return object.ReferenceEquals(receiverRef, receiver);
                }
                else
                {
                    return false;
                }
            });
        }
    }

    /// <summary>
    /// 全ての登録済み受信側クラスの削除用メソッド
    /// </summary>
    public void UnRegisterAll()
    {
        lock (this._lockObject)
        {
            this._receivers.Clear();
        }
    }

    /// <summary>
    /// メッセージ送信用メソッド
    /// </summary>
    /// <param name="parameter">メッセージパラメータ</param>
    public void Send(T parameter)
    {
        lock (this._lockObject)
        {
            bool isAlive;
            bool filtterResult;

            foreach (var element in this._receivers)
            {
                isAlive = element.Receiver.TryGetTarget(out var receiver);
                filtterResult = element.Filter is null ? true : element.Filter(parameter);

                // 参照が生きていてフィルタリングに合格したらメッセージ送信
                if (isAlive && filtterResult)
                {
                    Debug.Assert(receiver is not null);
                    receiver.ReciveMessage(parameter);
                }
            }
        }
    }

    /// <summary>
    /// 登録済みIReciverの数をカウント
    /// </summary>
    /// <returns>登録済みIReciverの数</returns>
    public int RegisteredCount()
    {
        return this._receivers.Count;
    }
}