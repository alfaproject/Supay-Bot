using System;
using System.Collections.Generic;
using System.Xml;

namespace Supay.Bot
{
  /// <summary>
  /// A structure to hold the RSS Feed items
  /// </summary>
  [Serializable]
  internal struct RssItem
  {
    /// <summary>
    /// The publishing date.
    /// </summary>
    public DateTime Date;

    /// <summary>
    /// A description of the content (or the feed itself)
    /// </summary>
    public string Description;

    /// <summary>
    /// The link to the feed
    /// </summary>
    public string Link;

    /// <summary>
    /// The title of the feed
    /// </summary>
    public string Title;
  }

  /// <summary>
  /// Class to parse and display RSS Feeds
  /// </summary>
  [Serializable]
  internal class RssManager : IDisposable
  {
    #region Variables

    private readonly List<RssItem> _rssItems = new List<RssItem>();
    private bool _IsDisposed;
    private string _feedDescription;
    private string _feedTitle;
    private string _url;

    #endregion

    #region Constructors

    /// <summary>
    /// Empty constructor, allowing us to
    /// instantiate our class and set our
    /// _url variable to an empty string
    /// </summary>
    public RssManager()
    {
      this._url = string.Empty;
    }

    /// <summary>
    /// Constructor allowing us to instantiate our class
    /// and set the _url variable to a value
    /// </summary>
    /// <param name="feedUrl">The URL of the Rss feed</param>
    public RssManager(string feedUrl)
    {
      this._url = feedUrl;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the URL of the RSS feed to parse.
    /// </summary>
    public string Url
    {
      get
      {
        return this._url;
      }
      set
      {
        this._url = value;
      }
    }

    /// <summary>
    /// Gets all the items in the RSS feed.
    /// </summary>
    public List<RssItem> RssItems
    {
      get
      {
        return this._rssItems;
      }
    }

    /// <summary>
    /// Gets the title of the RSS feed.
    /// </summary>
    public string FeedTitle
    {
      get
      {
        return this._feedTitle;
      }
    }

    /// <summary>
    /// Gets the description of the RSS feed.
    /// </summary>
    public string FeedDescription
    {
      get
      {
        return this._feedDescription;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Retrieves the remote RSS feed and parses it.
    /// </summary>
    public List<RssItem> GetFeed()
    {
      if (string.IsNullOrEmpty(this.Url))
      {
        throw new ArgumentException("You must provide a feed URL");
      }
      try
      {
        // start the parsing process
        using (XmlReader reader = XmlReader.Create(this.Url))
        {
          var xmlDoc = new XmlDocument();
          xmlDoc.Load(reader);

          // parse the items of the feed
          ParseDocElements(xmlDoc.SelectSingleNode("//channel"), "title", ref this._feedTitle);
          ParseDocElements(xmlDoc.SelectSingleNode("//channel"), "description", ref this._feedDescription);
          this.ParseRssItems(xmlDoc);
          return this._rssItems;
        }
      }
      catch
      {
        return new List<RssItem>();
      }
    }

    /// <summary>
    /// Parses the xml document in order to retrieve the RSS items.
    /// </summary>
    private void ParseRssItems(XmlDocument xmlDoc)
    {
      this._rssItems.Clear();
      XmlNodeList nodes = xmlDoc.SelectNodes("rss/channel/item");
      foreach (XmlNode node in nodes)
      {
        var item = new RssItem();
        ParseDocElements(node, "title", ref item.Title);
        ParseDocElements(node, "description", ref item.Description);
        ParseDocElements(node, "link", ref item.Link);

        string date = null;
        ParseDocElements(node, "pubDate", ref date);
        DateTime.TryParse(date, out item.Date);

        this._rssItems.Add(item);
      }
    }

    /// <summary>
    /// Parses the XmlNode with the specified XPath query 
    /// and assigns the value to the property parameter.
    /// </summary>
    private static void ParseDocElements(XmlNode parent, string xPath, ref string property)
    {
      XmlNode node = parent.SelectSingleNode(xPath);
      property = node != null ? node.InnerText : "Unresolvable";
    }

    #endregion

    #region IDisposable Members

    /// <summary>
    /// Releases the object to the garbage collector
    /// </summary>
    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    #endregion

    /// <summary>
    /// Performs the disposal.
    /// </summary>
    private void Dispose(bool disposing)
    {
      if (disposing && !this._IsDisposed)
      {
        this._rssItems.Clear();
        this._url = null;
        this._feedTitle = null;
        this._feedDescription = null;
      }

      this._IsDisposed = true;
    }
  }
}
