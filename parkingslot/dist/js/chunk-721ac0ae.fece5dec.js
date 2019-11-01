(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["chunk-721ac0ae"],{"0798":function(t,e,s){"use strict";s("0c18");var r=s("10d2"),i=s("afdd"),o=s("9d26"),a=s("f2e7"),n=s("7560"),l=s("2b0e"),c=l["a"].extend({name:"transitionable",props:{mode:String,origin:String,transition:String}}),d=s("58df"),u=s("d9bd");e["a"]=Object(d["a"])(r["a"],a["a"],c).extend({name:"v-alert",props:{border:{type:String,validator(t){return["top","right","bottom","left"].includes(t)}},closeLabel:{type:String,default:"$vuetify.close"},coloredBorder:Boolean,dense:Boolean,dismissible:Boolean,icon:{default:"",type:[Boolean,String],validator(t){return"string"===typeof t||!1===t}},outlined:Boolean,prominent:Boolean,text:Boolean,type:{type:String,validator(t){return["info","error","success","warning"].includes(t)}},value:{type:Boolean,default:!0}},computed:{__cachedBorder(){if(!this.border)return null;let t={staticClass:"v-alert__border",class:{[`v-alert__border--${this.border}`]:!0}};return this.coloredBorder&&(t=this.setBackgroundColor(this.computedColor,t),t.class["v-alert__border--has-color"]=!0),this.$createElement("div",t)},__cachedDismissible(){if(!this.dismissible)return null;const t=this.iconColor;return this.$createElement(i["a"],{staticClass:"v-alert__dismissible",props:{color:t,icon:!0,small:!0},attrs:{"aria-label":this.$vuetify.lang.t(this.closeLabel)},on:{click:()=>this.isActive=!1}},[this.$createElement(o["a"],{props:{color:t}},"$cancel")])},__cachedIcon(){return this.computedIcon?this.$createElement(o["a"],{staticClass:"v-alert__icon",props:{color:this.iconColor}},this.computedIcon):null},classes(){const t={...r["a"].options.computed.classes.call(this),"v-alert--border":Boolean(this.border),"v-alert--dense":this.dense,"v-alert--outlined":this.outlined,"v-alert--prominent":this.prominent,"v-alert--text":this.text};return this.border&&(t[`v-alert--border-${this.border}`]=!0),t},computedColor(){return this.color||this.type},computedIcon(){return!1!==this.icon&&("string"===typeof this.icon&&this.icon?this.icon:!!["error","info","success","warning"].includes(this.type)&&`$${this.type}`)},hasColoredIcon(){return this.hasText||Boolean(this.border)&&this.coloredBorder},hasText(){return this.text||this.outlined},iconColor(){return this.hasColoredIcon?this.computedColor:void 0},isDark(){return!(!this.type||this.coloredBorder||this.outlined)||n["a"].options.computed.isDark.call(this)}},created(){this.$attrs.hasOwnProperty("outline")&&Object(u["a"])("outline","outlined",this)},methods:{genWrapper(){const t=[this.$slots.prepend||this.__cachedIcon,this.genContent(),this.__cachedBorder,this.$slots.append,this.$scopedSlots.close?this.$scopedSlots.close({toggle:this.toggle}):this.__cachedDismissible],e={staticClass:"v-alert__wrapper"};return this.$createElement("div",e,t)},genContent(){return this.$createElement("div",{staticClass:"v-alert__content"},this.$slots.default)},genAlert(){let t={staticClass:"v-alert",attrs:{role:"alert"},class:this.classes,style:this.styles,directives:[{name:"show",value:this.isActive}]};if(!this.coloredBorder){const e=this.hasText?this.setTextColor:this.setBackgroundColor;t=e(this.computedColor,t)}return this.$createElement("div",t,[this.genWrapper()])},toggle(){this.isActive=!this.isActive}},render(t){const e=this.genAlert();return this.transition?t("transition",{props:{name:this.transition,origin:this.origin,mode:this.mode}},[e]):e}})},"0c18":function(t,e,s){},"0e8f":function(t,e,s){"use strict";s("20f6");var r=s("e8f2");e["a"]=Object(r["a"])("flex")},a523:function(t,e,s){"use strict";s("20f6"),s("4b85");var r=s("e8f2"),i=s("d9f7");e["a"]=Object(r["a"])("container").extend({name:"v-container",functional:!0,props:{id:String,tag:{type:String,default:"div"},fluid:{type:Boolean,default:!1}},render(t,{props:e,data:s,children:r}){let o;const{attrs:a}=s;return a&&(s.attrs={},o=Object.keys(a).filter(t=>{if("slot"===t)return!1;const e=a[t];return t.startsWith("data-")?(s.attrs[t]=e,!1):e||"string"===typeof e})),e.id&&(s.domProps=s.domProps||{},s.domProps.id=e.id),t(e.tag,Object(i["a"])(s,{staticClass:"container",class:Array({"container--fluid":e.fluid}).concat(o||[])}),r)}})},d860:function(t,e,s){"use strict";s.r(e);var r=function(){var t=this,e=t.$createElement,s=t._self._c||e;return s("v-container",{attrs:{"fill-height":""}},[s("v-layout",{attrs:{"align-center":"","justify-center":""}},[s("v-flex",{staticStyle:{overflow:"auto"},attrs:{xs12:"",sm8:"",md4:""}},[s("v-card",[s("v-card-text",[s("form",{on:{submit:function(e){return e.preventDefault(),t.getResetEmail(e)}}},[s("v-container",[s("v-layout",{attrs:{row:""}},[s("v-flex",{attrs:{xs12:""}},[s("v-alert",{attrs:{dismissible:"",type:"success",value:t.success}},[t._v("Check your email ("+t._s(t.email)+") to reset your password!")]),s("v-alert",{attrs:{dismissible:"",type:"error",value:t.error}},[t._v("The email is invalid")]),s("h3",[t._v("Forgot your password?")]),s("v-text-field",{attrs:{name:"username",label:"Enter Email",id:"username",type:"text",required:""},model:{value:t.email,callback:function(e){t.email=e},expression:"email"}})],1)],1)],1),s("v-layout",[s("v-flex",{attrs:{xs12:""}},[s("v-btn",{attrs:{color:"primary",type:"submit"}},[t._v("Reset Password")]),s("v-btn",{staticClass:"ml-5",attrs:{color:"primary",type:"submit",to:"/main"}},[t._v("Back")])],1)],1)],1)])],1)],1)],1)],1)},i=[],o={data(){return{email:"",error:!1,success:!1}},methods:{getResetEmail(){this.axios.post("https://parkingslotapi.azurewebsites.net/api/users/resetpassword",{email:this.email}).then(t=>{"200"==t.status?(this.success=!0,this.error=!1):(this.success=!1,this.error=!0)})}}},a=o,n=s("2877"),l=s("6544"),c=s.n(l),d=s("0798"),u=s("8336"),h=s("b0af"),p=s("99d9"),m=s("a523"),v=s("0e8f"),f=s("a722"),b=s("8654"),g=Object(n["a"])(a,r,i,!1,null,null,null);e["default"]=g.exports;c()(g,{VAlert:d["a"],VBtn:u["a"],VCard:h["a"],VCardText:p["b"],VContainer:m["a"],VFlex:v["a"],VLayout:f["a"],VTextField:b["a"]})}}]);
//# sourceMappingURL=chunk-721ac0ae.fece5dec.js.map