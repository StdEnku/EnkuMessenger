# ![logo](./img/logo.png)  
[![Build&Test](https://github.com/StdEnku/EnkuMessenger/actions/workflows/Build&Test.yml/badge.svg)](https://github.com/StdEnku/EnkuMessenger/actions/workflows/Build&Test.yml)
## 概要

本ライブラリ通常では情報のやり取りが行う事の難しい
MVVMパターンでいうところのViewModel-ViewModel間やView-ViewModel間での
情報のやり取りを行うためのメッセンジャーパターンと呼ばれる
デザインパターンの実装を提供するライブラリである。

受信側のクラスの参照は弱参照で保持されるのでメモリリークの心配はありません。

## 通常の通信

```c#
using EnkuMessenger;

// メッセージでやり取りされるオブジェクトの型
class MyMessage
{
    public string Value = string.Empty;
}

// 受信側クラス
class MyReceiver : IReceiver<MyMessage>
{
    public string Value = string.Empty;

    // メッセージ受信時の処理
    public void ReciveMessage(MyMessage message)
    {
        this.Value = message.Value;
    }
}

class Program
{ 
    static void Main()
    {
        // Messengerの用意
        IMessenger<MyMessage> messenger = Messenger<MyMessage>.Instance;

        // ダミーの受信側用意
        var myReceiver = new MyReceiver();
        messenger.Register(myReceiver);

        // ダミーのメッセージの用意と送信
        var myMessage = new MyMessage();
        myMessage.Value = "Hello World";
        messenger.Send(myMessage);

        Console.WriteLine(myReceiver.Value);
    }
}
```

## フィルターを用いたメッセージの選別

```c#
using EnkuMessenger;

// メッセージでやり取りされるオブジェクトの型
class MyMessage
{
    public string Key = string.Empty;
    public string Value = string.Empty;
}

// 受信側クラス
class MyReceiver : IReceiver<MyMessage>
{
    public string Value = "No Received";

    // メッセージ受信時の処理
    public void ReciveMessage(MyMessage message)
    {
        this.Value = message.Value;
    }
}

class Program
{ 
    static void Main()
    {
        const string KEY1 = "MsgKey1";
        const string KEY2 = "MsgKey2";

        // Messengerの用意
        IMessenger<MyMessage> messenger = Messenger<MyMessage>.Instance;

        // ダミーの受信側用意とフィルタ設定
        var myReceiver1 = new MyReceiver();
        var myReceiver2 = new MyReceiver();
        messenger.Register(myReceiver1, message => message.Key == KEY1);
        messenger.Register(myReceiver2, message => message.Key == KEY2);

        // ダミーのメッセージの用意と送信
        var myMessage = new MyMessage();
        myMessage.Value = "Received!!!";
        myMessage.Key = KEY1;
        messenger.Send(myMessage);

        Console.WriteLine($"myReceiver1.Value = {myReceiver1.Value}");
        Console.WriteLine($"myReceiver2.Value = {myReceiver2.Value}");
        // [result]
        // myReceiver1.Value = Received!!!
        // myReceiver2.Value = No Received
    }
}
```
