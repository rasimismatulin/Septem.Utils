using System;
using System.Collections.Generic;
using System.Linq;

namespace Septem.Notifications.Abstractions;

public class Receiver
{

    private readonly Dictionary<string, ICollection<string>> _internalParameters;


    public Guid? TargetUid { get; private set; }

    public string Token { get; private set; }

    public IReadOnlyDictionary<string, ICollection<string>> Parameters => _internalParameters;

    public ReceiverType ReceiverType { get; private set; }

    public Receiver(ReceiverType receiverType)
    {
        ReceiverType = receiverType;
        _internalParameters = new Dictionary<string, ICollection<string>>();
    }

    public Receiver(ReceiverType receiverType, Dictionary<string, ICollection<string>> dictionary)
    {
        ReceiverType = receiverType;
        _internalParameters = dictionary;
    }

    #region Extensions

    public static Receiver Sms() => new(ReceiverType.Sms);
    public static Receiver Fcm() => new(ReceiverType.Fcm);
    public static Receiver Email() => new(ReceiverType.Email);
    public static Receiver FcmOrSms() => new(ReceiverType.FcmOrSms);


    #endregion

    public Receiver WithTarget(Guid uid)
    {
        TargetUid = uid;
        return this;
    }

    public Receiver WithToken(string token)
    {
        Token = token;
        return this;
    }

    public Receiver WithParameter(string name, string value)
    {
        if (!Parameters.ContainsKey(name))
            _internalParameters[name] = new List<string> { value };
        else
            _internalParameters[name].Add(value);
        return this;
    }

    public Receiver WithParameters(string name, ICollection<string> values)
    {
        if (!Parameters.ContainsKey(name))
            _internalParameters[name] = new List<string>(values);
        else
            foreach (var value in values)
                _internalParameters[name].Add(value);
        return this;
    }

    public Receiver WithParameters(string name, params string[] values)
    {
        if (!Parameters.ContainsKey(name))
            _internalParameters[name] = new List<string>(values);
        else
            foreach (var value in values)
                _internalParameters[name].Add(value);
        return this;
    }

    public ICollection<Receiver> WithTokens(IEnumerable<string> tokens) => tokens.Select(token => new Receiver(ReceiverType).WithToken(token)).ToArray();
    public ICollection<Receiver> WithTokens(params string[] tokens) => WithTokens(tokens.AsEnumerable());
    public ICollection<Receiver> WithTargets(IEnumerable<Guid> targets) => targets.Select(target => new Receiver(ReceiverType).WithTarget(target)).ToArray();
    public ICollection<Receiver> WithTargets(params Guid[] targets) => WithTargets(targets.AsEnumerable());


    public string GetParameter(string name)
    {
        if (_internalParameters.ContainsKey(name))
            return _internalParameters[name].FirstOrDefault();
        return default;
    }

    public ICollection<string> GetParameters(string name)
    {
        if (_internalParameters.ContainsKey(name))
            return _internalParameters[name];
        return default;
    }
}