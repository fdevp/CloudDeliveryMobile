﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CloudDeliveryMobile.Tests.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class TestsResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal TestsResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("CloudDeliveryMobile.Tests.Resources.TestsResources", typeof(TestsResources).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to password.
        /// </summary>
        internal static string grant_type {
            get {
                return ResourceManager.GetString("grant_type", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to test.
        /// </summary>
        internal static string login {
            get {
                return ResourceManager.GetString("login", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to test.
        /// </summary>
        internal static string password {
            get {
                return ResourceManager.GetString("password", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {
        ///    &quot;access_token&quot;: &quot;token&quot;,
        ///    &quot;token_type&quot;: &quot;bearer&quot;,
        ///    &quot;expires_in&quot;: 1209599,
        ///    &quot;Login&quot;: &quot;login&quot;,
        ///    &quot;Roles&quot;: &quot;[\&quot;carrier\&quot;]&quot;,
        ///    &quot;Name&quot;: &quot;user&quot;,
        ///    &quot;.issued&quot;: &quot;Fri, 02 Mar 2018 13:02:42 GMT&quot;,
        ///    &quot;.expires&quot;: &quot;Fri, 16 Mar 2018 13:02:42 GMT&quot;
        ///}.
        /// </summary>
        internal static string singin_response {
            get {
                return ResourceManager.GetString("singin_response", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to token.
        /// </summary>
        internal static string token {
            get {
                return ResourceManager.GetString("token", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {
        ///    &quot;Login&quot;: &quot;userlogin&quot;,
        ///    &quot;Name&quot;: &quot;username&quot;,
        ///    &quot;HasRegistered&quot;: true,
        ///    &quot;LoginProvider&quot;: null,
        ///    &quot;Roles&quot;: &quot;[\&quot;carrier\&quot;]&quot;
        ///}.
        /// </summary>
        internal static string token_singin_response {
            get {
                return ResourceManager.GetString("token_singin_response", resourceCulture);
            }
        }
    }
}
