namespace RuneScript {
    
    
    /// <remarks/>
    [System.Web.Services.WebServiceBinding(Name="RScriptLookupPort", Namespace="urn:RScriptLookup")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class RScriptLookup : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback statsOperationCompleted;
        
        private System.Threading.SendOrPostCallback trackGetTimeAllOperationCompleted;
        
        private System.Threading.SendOrPostCallback trackGetTimeAgoAllOperationCompleted;
        
        private System.Threading.SendOrPostCallback trackUpdateUserOperationCompleted;
        
        public RScriptLookup() {
            this.Url = "http://rscript.org/lookup.php?soap";
        }
        
        public event statsCompletedEventHandler statsCompleted;
        
        public event trackGetTimeAllCompletedEventHandler trackGetTimeAllCompleted;
        
        public event trackGetTimeAgoAllCompletedEventHandler trackGetTimeAgoAllCompleted;
        
        public event trackUpdateUserCompletedEventHandler trackUpdateUserCompleted;
        
        /// <remarks>
///Retreaves the stats of a User. Name must be under 12 characters.
///</remarks>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("urn:RScriptLookup#stats", RequestNamespace="urn:RScriptLookup", ResponseNamespace="urn:RScriptLookup")]
        [return: System.Xml.Serialization.SoapElement("return")]
        public skills stats(string name) {
            object[] results = this.Invoke("stats", new object[] {
                        name});
            return ((skills)(results[0]));
        }
        
        public System.IAsyncResult Beginstats(string name, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("stats", new object[] {
                        name}, callback, asyncState);
        }
        
        public skills Endstats(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((skills)(results[0]));
        }
        
        public void statsAsync(string name) {
            this.statsAsync(name, null);
        }
        
        public void statsAsync(string name, object userState) {
            if ((this.statsOperationCompleted == null)) {
                this.statsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnstatsCompleted);
            }
            this.InvokeAsync("stats", new object[] {
                        name}, this.statsOperationCompleted, userState);
        }
        
        private void OnstatsCompleted(object arg) {
            if ((this.statsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.statsCompleted(this, new statsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks>
///Gets a single tracker entry of all skills at a specific time [use a unix time stamp]. Does not include starting details.
///</remarks>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("urn:RScriptLookup#trackGetTimeAll", RequestNamespace="urn:RScriptLookup", ResponseNamespace="urn:RScriptLookup")]
        [return: System.Xml.Serialization.SoapElement("return")]
        public skills trackGetTimeAll(string name, int time) {
            object[] results = this.Invoke("trackGetTimeAll", new object[] {
                        name,
                        time});
            return ((skills)(results[0]));
        }
        
        public System.IAsyncResult BegintrackGetTimeAll(string name, int time, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("trackGetTimeAll", new object[] {
                        name,
                        time}, callback, asyncState);
        }
        
        public skills EndtrackGetTimeAll(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((skills)(results[0]));
        }
        
        public void trackGetTimeAllAsync(string name, int time) {
            this.trackGetTimeAllAsync(name, time, null);
        }
        
        public void trackGetTimeAllAsync(string name, int time, object userState) {
            if ((this.trackGetTimeAllOperationCompleted == null)) {
                this.trackGetTimeAllOperationCompleted = new System.Threading.SendOrPostCallback(this.OntrackGetTimeAllCompleted);
            }
            this.InvokeAsync("trackGetTimeAll", new object[] {
                        name,
                        time}, this.trackGetTimeAllOperationCompleted, userState);
        }
        
        private void OntrackGetTimeAllCompleted(object arg) {
            if ((this.trackGetTimeAllCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.trackGetTimeAllCompleted(this, new trackGetTimeAllCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks>
///Gets a single tracker entry made x seconds ago. Does not include starting details.
///</remarks>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("urn:RScriptLookup#trackGetAgoAll", RequestNamespace="urn:RScriptLookup", ResponseNamespace="urn:RScriptLookup")]
        [return: System.Xml.Serialization.SoapElement("return")]
        public skills trackGetTimeAgoAll(string name, int time) {
            object[] results = this.Invoke("trackGetTimeAgoAll", new object[] {
                        name,
                        time});
            return ((skills)(results[0]));
        }
        
        public System.IAsyncResult BegintrackGetTimeAgoAll(string name, int time, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("trackGetTimeAgoAll", new object[] {
                        name,
                        time}, callback, asyncState);
        }
        
        public skills EndtrackGetTimeAgoAll(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((skills)(results[0]));
        }
        
        public void trackGetTimeAgoAllAsync(string name, int time) {
            this.trackGetTimeAgoAllAsync(name, time, null);
        }
        
        public void trackGetTimeAgoAllAsync(string name, int time, object userState) {
            if ((this.trackGetTimeAgoAllOperationCompleted == null)) {
                this.trackGetTimeAgoAllOperationCompleted = new System.Threading.SendOrPostCallback(this.OntrackGetTimeAgoAllCompleted);
            }
            this.InvokeAsync("trackGetTimeAgoAll", new object[] {
                        name,
                        time}, this.trackGetTimeAgoAllOperationCompleted, userState);
        }
        
        private void OntrackGetTimeAgoAllCompleted(object arg) {
            if ((this.trackGetTimeAgoAllCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.trackGetTimeAgoAllCompleted(this, new trackGetTimeAgoAllCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks>
///Updates a user's stats by quering the hiscores.
///</remarks>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("urn:RScriptLookup#trackUpdateUser", RequestNamespace="urn:RScriptLookup", ResponseNamespace="urn:RScriptLookup")]
        [return: System.Xml.Serialization.SoapElement("return")]
        public bool trackUpdateUser(string name) {
            object[] results = this.Invoke("trackUpdateUser", new object[] {
                        name});
            return ((bool)(results[0]));
        }
        
        public System.IAsyncResult BegintrackUpdateUser(string name, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("trackUpdateUser", new object[] {
                        name}, callback, asyncState);
        }
        
        public bool EndtrackUpdateUser(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((bool)(results[0]));
        }
        
        public void trackUpdateUserAsync(string name) {
            this.trackUpdateUserAsync(name, null);
        }
        
        public void trackUpdateUserAsync(string name, object userState) {
            if ((this.trackUpdateUserOperationCompleted == null)) {
                this.trackUpdateUserOperationCompleted = new System.Threading.SendOrPostCallback(this.OntrackUpdateUserCompleted);
            }
            this.InvokeAsync("trackUpdateUser", new object[] {
                        name}, this.trackUpdateUserOperationCompleted, userState);
        }
        
        private void OntrackUpdateUserCompleted(object arg) {
            if ((this.trackUpdateUserCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.trackUpdateUserCompleted(this, new trackUpdateUserCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.1433")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapType(Namespace="urn:RScriptLookup")]
    public partial class skills {
        
        private skillVar overallField;
        
        private skillVar attackField;
        
        private skillVar defenceField;
        
        private skillVar strengthField;
        
        private skillVar hitpointsField;
        
        private skillVar rangedField;
        
        private skillVar prayerField;
        
        private skillVar magicField;
        
        private skillVar cookingField;
        
        private skillVar woodcuttingField;
        
        private skillVar fletchingField;
        
        private skillVar fishingField;
        
        private skillVar firemakingField;
        
        private skillVar craftingField;
        
        private skillVar smithingField;
        
        private skillVar miningField;
        
        private skillVar herbloreField;
        
        private skillVar agilityField;
        
        private skillVar thievingField;
        
        private skillVar slayerField;
        
        private skillVar farmingField;
        
        private skillVar runecraftField;
        
        private skillVar hunterField;
        
        private skillVar constructionField;
        
        private skillVar summoningField;
        
        /// <remarks/>
        public skillVar overall {
            get {
                return this.overallField;
            }
            set {
                this.overallField = value;
            }
        }
        
        /// <remarks/>
        public skillVar attack {
            get {
                return this.attackField;
            }
            set {
                this.attackField = value;
            }
        }
        
        /// <remarks/>
        public skillVar defence {
            get {
                return this.defenceField;
            }
            set {
                this.defenceField = value;
            }
        }
        
        /// <remarks/>
        public skillVar strength {
            get {
                return this.strengthField;
            }
            set {
                this.strengthField = value;
            }
        }
        
        /// <remarks/>
        public skillVar hitpoints {
            get {
                return this.hitpointsField;
            }
            set {
                this.hitpointsField = value;
            }
        }
        
        /// <remarks/>
        public skillVar ranged {
            get {
                return this.rangedField;
            }
            set {
                this.rangedField = value;
            }
        }
        
        /// <remarks/>
        public skillVar prayer {
            get {
                return this.prayerField;
            }
            set {
                this.prayerField = value;
            }
        }
        
        /// <remarks/>
        public skillVar magic {
            get {
                return this.magicField;
            }
            set {
                this.magicField = value;
            }
        }
        
        /// <remarks/>
        public skillVar cooking {
            get {
                return this.cookingField;
            }
            set {
                this.cookingField = value;
            }
        }
        
        /// <remarks/>
        public skillVar woodcutting {
            get {
                return this.woodcuttingField;
            }
            set {
                this.woodcuttingField = value;
            }
        }
        
        /// <remarks/>
        public skillVar fletching {
            get {
                return this.fletchingField;
            }
            set {
                this.fletchingField = value;
            }
        }
        
        /// <remarks/>
        public skillVar fishing {
            get {
                return this.fishingField;
            }
            set {
                this.fishingField = value;
            }
        }
        
        /// <remarks/>
        public skillVar firemaking {
            get {
                return this.firemakingField;
            }
            set {
                this.firemakingField = value;
            }
        }
        
        /// <remarks/>
        public skillVar crafting {
            get {
                return this.craftingField;
            }
            set {
                this.craftingField = value;
            }
        }
        
        /// <remarks/>
        public skillVar smithing {
            get {
                return this.smithingField;
            }
            set {
                this.smithingField = value;
            }
        }
        
        /// <remarks/>
        public skillVar mining {
            get {
                return this.miningField;
            }
            set {
                this.miningField = value;
            }
        }
        
        /// <remarks/>
        public skillVar herblore {
            get {
                return this.herbloreField;
            }
            set {
                this.herbloreField = value;
            }
        }
        
        /// <remarks/>
        public skillVar agility {
            get {
                return this.agilityField;
            }
            set {
                this.agilityField = value;
            }
        }
        
        /// <remarks/>
        public skillVar thieving {
            get {
                return this.thievingField;
            }
            set {
                this.thievingField = value;
            }
        }
        
        /// <remarks/>
        public skillVar slayer {
            get {
                return this.slayerField;
            }
            set {
                this.slayerField = value;
            }
        }
        
        /// <remarks/>
        public skillVar farming {
            get {
                return this.farmingField;
            }
            set {
                this.farmingField = value;
            }
        }
        
        /// <remarks/>
        public skillVar runecraft {
            get {
                return this.runecraftField;
            }
            set {
                this.runecraftField = value;
            }
        }
        
        /// <remarks/>
        public skillVar hunter {
            get {
                return this.hunterField;
            }
            set {
                this.hunterField = value;
            }
        }
        
        /// <remarks/>
        public skillVar construction {
            get {
                return this.constructionField;
            }
            set {
                this.constructionField = value;
            }
        }
        
        /// <remarks/>
        public skillVar summoning {
            get {
                return this.summoningField;
            }
            set {
                this.summoningField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.1433")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapType(Namespace="urn:RScriptLookup")]
    public partial class skillVar {
        
        private int rankField;
        
        private int expField;
        
        private int levelField;
        
        /// <remarks/>
        public int rank {
            get {
                return this.rankField;
            }
            set {
                this.rankField = value;
            }
        }
        
        /// <remarks/>
        public int exp {
            get {
                return this.expField;
            }
            set {
                this.expField = value;
            }
        }
        
        /// <remarks/>
        public int level {
            get {
                return this.levelField;
            }
            set {
                this.levelField = value;
            }
        }
    }
    
    public partial class statsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal statsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public skills Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((skills)(this.results[0]));
            }
        }
    }
    
    public delegate void statsCompletedEventHandler(object sender, statsCompletedEventArgs args);
    
    public partial class trackGetTimeAllCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal trackGetTimeAllCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public skills Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((skills)(this.results[0]));
            }
        }
    }
    
    public delegate void trackGetTimeAllCompletedEventHandler(object sender, trackGetTimeAllCompletedEventArgs args);
    
    public partial class trackGetTimeAgoAllCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal trackGetTimeAgoAllCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public skills Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((skills)(this.results[0]));
            }
        }
    }
    
    public delegate void trackGetTimeAgoAllCompletedEventHandler(object sender, trackGetTimeAgoAllCompletedEventArgs args);
    
    public partial class trackUpdateUserCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal trackUpdateUserCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public bool Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
    
    public delegate void trackUpdateUserCompletedEventHandler(object sender, trackUpdateUserCompletedEventArgs args);
}