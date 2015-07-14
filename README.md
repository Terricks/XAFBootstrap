# XAFBootstrap
XAF Bootstrap addon for eXpressApp Framework (WebForms)

# Installation
1. Download project
2. Create new or open existing XAF project
3. Add XAF Bootstrap module to Module.cs in .Module.Web
4. Install Microsoft Web Optimization Framework to your .Web project

You can see video: https://www.youtube.com/watch?v=mIZcLswlswM

#Requirements

Microsoft Web Optimization Framework

# Functionality

- Twitter Bootstrap implementation for eXpressApp Framework (over standard XAF controllers, templates and editors functionality)
- Detecting unsaved data when closing browser tab or navigating in application
- Custom popup managing functionality (all over main window)
- Default 16 bootstrap themes and theme importing functionality

# License
Apache 2.0

# Releasing
For correct access to optimized *.js and *.css files by optimization framework you shoud add to web.config of your .Web project follow strings:
<pre>
&lt;location path=&quot;bootstrap_css/css.aspx&quot;&gt;<br/> &lt;system.web&gt;<br/> &lt;authorization&gt;<br/> &lt;allow users=&quot;?&quot;/&gt;<br/> &lt;/authorization&gt;<br/> &lt;/system.web&gt;<br/>&lt;/location&gt;<br/>&lt;location path=&quot;bootstrap_js/js.aspx&quot;&gt;<br/> &lt;system.web&gt;<br/> &lt;authorization&gt;<br/> &lt;allow users=&quot;?&quot;/&gt;<br/> &lt;/authorization&gt;<br/> &lt;/system.web&gt;<br/>&lt;/location&gt;
</pre>
