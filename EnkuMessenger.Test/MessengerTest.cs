namespace EnkuMessenger.Test;

using Xunit;
using EnkuMessenger;
using System.Threading.Tasks;

class DummyMessage 
{
    public string Value = string.Empty;
    public string Key = string.Empty;
}

class DummyReceiver : IReceiver<DummyMessage>
{
    public string Value = string.Empty;
    public int ReciveNum = 0;

    public void ReciveMessage(DummyMessage message)
    {
        this.Value = message.Value;
        this.ReciveNum++;
    }
}

public class MessengerTest
{
    const string DUMMY_VALUE = nameof(DUMMY_VALUE);

    [Fact]
    public void 送受信機能_受信側が1つの場合での通信時_メッセージが正しく送受信される()
    {
        // Messengerの用意
        IMessenger<DummyMessage> messenger = Messenger<DummyMessage>.Instance;

        // ダミーの受信側用意
        var dummyReceiver = new DummyReceiver();
        messenger.Register(dummyReceiver);

        // ダミーのメッセージの用意と送信
        var dummyMessage = new DummyMessage();
        dummyMessage.Value = DUMMY_VALUE;
        messenger.Send(dummyMessage);

        // 判定
        Assert.Equal(DUMMY_VALUE, dummyReceiver.Value);

        // 終了処理
        messenger.UnRegister(dummyReceiver);
    }

    [Fact]
    public void 送受信機能_受信側の登録が解除されている場合での通信時_メッセージは受信されない()
    {
        // Messengerの用意
        IMessenger<DummyMessage> messenger = Messenger<DummyMessage>.Instance;

        // ダミーの受信側用意
        var dummyReceiver = new DummyReceiver();
        messenger.Register(dummyReceiver);
        messenger.UnRegister(dummyReceiver);

        // ダミーのメッセージの用意と送信
        var dummyMessage = new DummyMessage();
        dummyMessage.Value = DUMMY_VALUE;
        messenger.Send(dummyMessage);

        // 判定
        Assert.NotEqual(DUMMY_VALUE, dummyReceiver.Value);
    }

    [Fact]
    public void 送受信機能_受信側が5つの場合での通信時_メッセージが正しく送受信される()
    {
        // Messengerの用意
        IMessenger<DummyMessage> messenger = Messenger<DummyMessage>.Instance;

        // ダミーの受信側用意
        DummyReceiver[] dummyReceivers =
        new DummyReceiver[] {
            new DummyReceiver(),
            new DummyReceiver(),
            new DummyReceiver(),
            new DummyReceiver(),
            new DummyReceiver()
        };

        // 登録
        foreach (var dummyReceiver in dummyReceivers)
        {
            messenger.Register(dummyReceiver);
        }

        // ダミーのメッセージの用意と送信
        var dummyMessage = new DummyMessage();
        dummyMessage.Value = DUMMY_VALUE;
        messenger.Send(dummyMessage);

        // 判定
        foreach (var dummyReceiver in dummyReceivers)
        {
            Assert.Equal(DUMMY_VALUE, dummyReceiver.Value);
        }

        // 終了処理
        foreach (var dummyReceiver in dummyReceivers)
        {
            messenger.UnRegister(dummyReceiver);
        }
    }

    [Fact]
    public void 送受信機能_同一の受信側オブジェクトを複数回登録_メッセージは1度しか受信しない()
    {
        // Messengerの用意
        IMessenger<DummyMessage> messenger = Messenger<DummyMessage>.Instance;

        // ダミーの受信側用意
        var dummyReceiver = new DummyReceiver();
        messenger.Register(dummyReceiver);
        messenger.Register(dummyReceiver);
        messenger.Register(dummyReceiver);

        // ダミーのメッセージの用意と送信
        var dummyMessage = new DummyMessage();
        dummyMessage.Value = DUMMY_VALUE;
        messenger.Send(dummyMessage);

        // 判定
        Assert.Equal(1, dummyReceiver.ReciveNum);

        // 終了処理
        messenger.UnRegister(dummyReceiver);
    }

    [Fact]
    public void 送受信機能_フィルターにかからない受信側にメッセージを送信_受信されない()
    {
        // Messengerの用意
        IMessenger<DummyMessage> messenger = Messenger<DummyMessage>.Instance;

        // ダミーの受信側用意
        const string KEY = "hogehoge";
        var dummyReceiver = new DummyReceiver();
        messenger.Register(dummyReceiver, message => message.Key == KEY);

        // ダミーのメッセージの用意と送信
        var dummyMessage = new DummyMessage();
        dummyMessage.Key = "fugafuga";
        dummyMessage.Value = DUMMY_VALUE;
        messenger.Send(dummyMessage);

        // 判定
        Assert.NotEqual(DUMMY_VALUE, dummyReceiver.Value);

        // 終了処理
        messenger.UnRegister(dummyReceiver);
    }

    [Fact]
    public void 送受信機能_フィルターにかかる受信側にメッセージを送信_受信される()
    {
        // Messengerの用意
        IMessenger<DummyMessage> messenger = Messenger<DummyMessage>.Instance;

        // ダミーの受信側用意
        const string KEY = "hogehoge";
        var dummyReceiver = new DummyReceiver();
        messenger.Register(dummyReceiver, message => message.Key == KEY);

        // ダミーのメッセージの用意と送信
        var dummyMessage = new DummyMessage();
        dummyMessage.Key = KEY;
        dummyMessage.Value = DUMMY_VALUE;
        messenger.Send(dummyMessage);

        // 判定
        Assert.Equal(DUMMY_VALUE, dummyReceiver.Value);

        // 終了処理
        messenger.UnRegister(dummyReceiver);
    }

    [Fact]
    public void IsRegistered_登録済みの受信側を引数に渡す_trueを返す()
    {
        // Messengerの用意
        IMessenger<DummyMessage> messenger = Messenger<DummyMessage>.Instance;

        // ダミーの受信側用意
        var dummyReceiver = new DummyReceiver();
        messenger.Register(dummyReceiver);

        // 判定
        var result = messenger.IsRegistered(dummyReceiver);
        Assert.True(result);

        // 終了処理
        messenger.UnRegister(dummyReceiver);
    }

    [Fact]
    public void IsRegistered_解除済みの受信側を引数に渡す_falseを返す()
    {
        // Messengerの用意
        IMessenger<DummyMessage> messenger = Messenger<DummyMessage>.Instance;

        // ダミーの受信側用意
        var dummyReceiver = new DummyReceiver();
        messenger.Register(dummyReceiver);
        messenger.UnRegister(dummyReceiver);

        // 判定
        var result = messenger.IsRegistered(dummyReceiver);
        Assert.False(result);

        // 終了処理
        messenger.UnRegister(dummyReceiver);
    }

    [Fact]
    public void Register_大量の受信側をマルチスレッドで登録_適切な数の受信側が登録される()
    {
        // Messengerの用意
        IMessenger<DummyMessage> messenger = Messenger<DummyMessage>.Instance;

        const int THREAD_NUM = 500;

        Parallel.For(0, THREAD_NUM, i =>
        {
            var dummyReceiver = new DummyReceiver();
            messenger.Register(dummyReceiver);
        });

        var result = messenger.RegisteredCount();
        Assert.Equal(THREAD_NUM, result);

        messenger.UnRegisterAll();
    }
}