using System;
using System.Collections.Generic;
using System.Resources;
using Septem.Utils.Helpers.Extensions;

namespace Septem.Utils.Helpers.ActionInvoke;

public static class RequestExtensions
{
    public static ResourceManager IssueResourceManager;

    #region Success

    public static Result AsResult(this Request request) => new(request.Id);

    public static Result<T> AsResultOf<T>(this Request request, T payload) => new(request.Id, payload);

    public static ResultOfCollection<T> AsResultOfCollection<T>(this Request request, IEnumerable<T> collection) => new(request.Id, collection);

    #endregion

    #region Not found

    public static Result AsNotFound(this Request request, params string[] reasons) => 
        new(request.Id, new Issue(IssueOrigin.NotFound, reasons));

    public static Result AsNotFoundFormat(this Request request, string objectName, object objectKey) => 
        new(request.Id, new Issue(IssueOrigin.NotFound, string.Format(IssueResourceManager.GetString("EntityNotFound", request.CultureInfo) ?? "Not localized message", objectName, objectKey)));

    public static Result<T> AsNotFound<T>(this Request request, params string[] reasons) => 
        new(request.Id, new Issue(IssueOrigin.NotFound, reasons));

    public static Result<T> AsNotFoundFormat<T>(this Request request, string objectName, object objectKey) =>
        new(request.Id, new Issue(IssueOrigin.NotFound, string.Format(IssueResourceManager.GetString("EntityNotFound", request.CultureInfo) ?? "Not localized message", objectName, objectKey)));

    public static ResultOfCollection<T> AsNotFoundCollection<T>(this Request request, params string[] reasons) => 
        new(request.Id, new Issue(IssueOrigin.NotFound, reasons));

    public static ResultOfCollection<T> AsNotFoundCollection<T>(this Request request, string objectName, object objectKey) =>
        new(request.Id, new Issue(IssueOrigin.NotFound, string.Format(IssueResourceManager.GetString("EntityNotFound", request.CultureInfo) ?? "Not localized message", objectName, objectKey)));

    #endregion

    #region Controversy

    public static Result AsControversyFormat(this Request request, string message, params object[] arguments) =>
        AsControversy(request, string.Format(IssueResourceManager.GetString(message, request.CultureInfo) ?? "Not localized message", arguments));

    public static Result<T> AsControversyFormat<T>(this Request request, string message, params object[] arguments) =>
        AsControversy<T>(request, string.Format(IssueResourceManager.GetString(message, request.CultureInfo) ?? "Not localized message", arguments));

    public static Result AsControversy(this Request request, params string[] reasons) =>
        new(request.Id, new Issue(IssueOrigin.Controversy, reasons));

    public static Result<T> AsControversy<T>(this Request request, params string[] reasons) =>
        new(request.Id, new Issue(IssueOrigin.Controversy, reasons));

    public static Result<T> AsControversy<T>(this Request request, IEnumerable<string> reasons) =>
        new(request.Id, new Issue(IssueOrigin.Controversy, reasons));

    public static ResultOfCollection<T> AsCollectionControversy<T>(this Request request, params string[] reasons) =>
        new(request.Id, new Issue(IssueOrigin.Controversy, reasons));

    #endregion

    #region Exception

    public static Result AsException(this Request request, Exception ex) =>
        new(request.Id, new Issue(IssueOrigin.Unknown, ex.GetMessages()));

    public static Result<T> AsException<T>(this Request request, Exception ex) =>
        new(request.Id, new Issue(IssueOrigin.Unknown, ex.GetMessages()));

    public static ResultOfCollection<T> AsCollectionException<T>(this Request request, Exception ex) =>
        new(request.Id, new Issue(IssueOrigin.Unknown, ex.GetMessages()));

    #endregion

    #region IssueOrigin
    public static Result<T> WithIssueOriginFormat<T>(this Request request, IssueOrigin issueOrigin,
        string message, params object[] arguments) => 
        new(request.Id, new Issue(issueOrigin, string.Format(IssueResourceManager.GetString(message, request.CultureInfo) ?? "Not localized message", arguments)));

    public static Result WithIssueOrigin(this Request request, IssueOrigin issueOrigin, params string[] reasons) =>
        new(request.Id, new Issue(issueOrigin, reasons));

    public static Result<T> WithIssueOrigin<T>(this Request request, IssueOrigin issueOrigin,
        params string[] reasons) => new(request.Id, new Issue(issueOrigin, reasons));

    public static ResultOfCollection<T> WithCollectionIssueOrigin<T>(this Request request, IssueOrigin issueOrigin,
        params string[] reasons) => new(request.Id, new Issue(issueOrigin, reasons));

    #endregion

    public static TInitiated Initiate<TInitiated>(this Request request, Action<TInitiated> action = null) where TInitiated : Request, new()
    {
        var initiated = new TInitiated { InitiatorId = request.Id };
        action?.Invoke(initiated);
        return initiated;
    }
}