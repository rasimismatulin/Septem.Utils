using System;
using System.Collections.Generic;
using System.Linq;
using Septem.Utils.Helpers.Extensions;

namespace Septem.Utils.Helpers.ActionInvoke;

public class Issue
{
    public Issue(IssueOrigin origin, params string[] reasons)
    {
        if (origin == 0)
            throw new ArgumentException("origin == 0", nameof(origin));
        if (reasons == null)
            throw new ArgumentNullException(nameof(reasons));
        if (!reasons.Any())
            throw new ArgumentException("!reasons.Any()", nameof(reasons));

        Origin = origin;
        Reasons = reasons;
    }

    public Issue(IssueOrigin severity, IEnumerable<string> reasons) : this(severity, reasons.ToArray())
    {
    }

    public IssueOrigin Origin { get; }

    public ICollection<string> Reasons { get; }

    public override string ToString() => $"Issue -> [Origin: {Origin}] [Code: {(int)Origin}]\nReasons: [{Reasons.JoinStrings()}]";

    public object GetMessageForException() => $"\nInnerIssue -> [Origin: {Origin}] [Code: {(int)Origin}] Reasons: [{Reasons.JoinStrings()}]\n";
}