﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.5466
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GetTogether.Resource {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Language {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public Language() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("GetTogether.Resource.Language", typeof(Language).Assembly);
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
        ///   Looks up a localized string similar to Any.
        /// </summary>
        public static string Any {
            get {
                return ResourceManager.GetString("Any", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Operation System.
        /// </summary>
        public static string OperationSystem {
            get {
                return ResourceManager.GetString("OperationSystem", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Record not found!.
        /// </summary>
        public static string Record_not_found {
            get {
                return ResourceManager.GetString("Record_not_found", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (&lt;font color=&quot;red&quot;&gt;{0}&lt;/font&gt; Records Found).
        /// </summary>
        public static string RecordsFoundString {
            get {
                return ResourceManager.GetString("RecordsFoundString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Select a format.
        /// </summary>
        public static string SelectFormat {
            get {
                return ResourceManager.GetString("SelectFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Show {0} Record(s).
        /// </summary>
        public static string Show_Records {
            get {
                return ResourceManager.GetString("Show_Records", resourceCulture);
            }
        }
    }
}