﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Septem.Utils.Domain.FluentValidation {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class ValidationMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ValidationMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Septem.Utils.Domain.FluentValidation.ValidationMessages", typeof(ValidationMessages).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Entity with same properties already exists. Details: [Type = {0}; UID = {1}].
        /// </summary>
        public static string AlreadyExists {
            get {
                return ResourceManager.GetString("AlreadyExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Guest count can be from {0} up to {1}.
        /// </summary>
        public static string BookGuestCountError {
            get {
                return ResourceManager.GetString("BookGuestCountError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Date or time is in past. {0} -&gt; {1}.
        /// </summary>
        public static string DateInPast {
            get {
                return ResourceManager.GetString("DateInPast", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Date is out of range [Property = {0}; Value = {1}].
        /// </summary>
        public static string DateTimeOutOfRange {
            get {
                return ResourceManager.GetString("DateTimeOutOfRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to String of {0} must be {2} length.
        /// </summary>
        public static string ExactLength {
            get {
                return ResourceManager.GetString("ExactLength", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid input format for {0} -&gt; format: {2}.
        /// </summary>
        public static string FormatError {
            get {
                return ResourceManager.GetString("FormatError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Value of &apos;{0}&apos; must be greater than {2}.
        /// </summary>
        public static string GreaterThan {
            get {
                return ResourceManager.GetString("GreaterThan", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Value of &apos;{0}&apos; must be greater or equals {2}.
        /// </summary>
        public static string GreaterThanOrEquals {
            get {
                return ResourceManager.GetString("GreaterThanOrEquals", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid entity relation. Relation: {1}; Current: {3}; Expected: {2}; .
        /// </summary>
        public static string InvalidEntityRelation {
            get {
                return ResourceManager.GetString("InvalidEntityRelation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid week day index.
        /// </summary>
        public static string InvalidWeekDayIndex {
            get {
                return ResourceManager.GetString("InvalidWeekDayIndex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enum value is out of range. [Enum = {0}].
        /// </summary>
        public static string IsInEnum {
            get {
                return ResourceManager.GetString("IsInEnum", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Value of &apos;{0}&apos; must be less than {2}.
        /// </summary>
        public static string LessThan {
            get {
                return ResourceManager.GetString("LessThan", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Value of &apos;{0}&apos; must be less or equals {2}.
        /// </summary>
        public static string LessThanOrEquals {
            get {
                return ResourceManager.GetString("LessThanOrEquals", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The length of &apos;{0}&apos; must be {2} characters or fewer..
        /// </summary>
        public static string MaximumLength {
            get {
                return ResourceManager.GetString("MaximumLength", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {2} must be grather than {3}.
        /// </summary>
        public static string MaxMinError {
            get {
                return ResourceManager.GetString("MaxMinError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Guest count can be from {0}.
        /// </summary>
        public static string MinimumBookGuestCountError {
            get {
                return ResourceManager.GetString("MinimumBookGuestCountError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The length of &apos;{0}&apos; must be at least {2} characters..
        /// </summary>
        public static string MinimumLength {
            get {
                return ResourceManager.GetString("MinimumLength", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; must not be empty..
        /// </summary>
        public static string NotEmpty {
            get {
                return ResourceManager.GetString("NotEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Entity not exists. Details: [Type = {0}; UID = {1}].
        /// </summary>
        public static string NotFound {
            get {
                return ResourceManager.GetString("NotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; must not be null..
        /// </summary>
        public static string NotNull {
            get {
                return ResourceManager.GetString("NotNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to sort collection by &apos;{0}&apos; .
        /// </summary>
        public static string OrderByIsNotAvailable {
            get {
                return ResourceManager.GetString("OrderByIsNotAvailable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; is in invalid format.
        /// </summary>
        public static string RegexError {
            get {
                return ResourceManager.GetString("RegexError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This operation can repeat in {0} seconds.
        /// </summary>
        public static string RepeatTimeLimit {
            get {
                return ResourceManager.GetString("RepeatTimeLimit", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This table already have book [Start: {0:HH:mm:ss} -&gt; End: {1:HH:mm:ss}].
        /// </summary>
        public static string TableBookIntersection {
            get {
                return ResourceManager.GetString("TableBookIntersection", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Table not selected.
        /// </summary>
        public static string TableNotSelected {
            get {
                return ResourceManager.GetString("TableNotSelected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Time is out of range [Property = {0}].
        /// </summary>
        public static string TimeOutOfRange {
            get {
                return ResourceManager.GetString("TimeOutOfRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid time entered; Start: {1}; End: {2};.
        /// </summary>
        public static string TimeSheetValidation {
            get {
                return ResourceManager.GetString("TimeSheetValidation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User already registered.
        /// </summary>
        public static string UserAlreadyRegistered {
            get {
                return ResourceManager.GetString("UserAlreadyRegistered", resourceCulture);
            }
        }
    }
}
