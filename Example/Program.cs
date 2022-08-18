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
    }
}