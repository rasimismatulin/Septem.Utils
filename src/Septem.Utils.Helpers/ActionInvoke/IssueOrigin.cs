namespace Septem.Utils.Helpers.ActionInvoke;

public enum IssueOrigin
{
    UserInput = 1,
    NotFound = 2,
    Controversy = 3,
    ExternalFailure = 4,
    ExternalTimeout = 5,
    Unknown = 6,
    InvalidPassword = 7
}