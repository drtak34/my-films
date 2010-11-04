using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Reflection;
using System.IO;


class XmlConf
{
    
    //Listes qui contiennent tous les noeuds
    public List<ListNode> listGen = new List<ListNode>();
    public List<ListNode> listSearch = new List<ListNode>();
    public List<ListNode> listDetail = new List<ListNode>();

    //Contructeur
    public XmlConf(string configFile)
    {
        if (File.Exists(configFile))
            init(configFile);
        else
            initNew(configFile);
    }

    public void initNew(string configFile)
    {

        Assembly _ass = Assembly.GetExecutingAssembly();
        Stream _stream = _ass.GetManifestResourceStream(_ass.GetName().Name + ".MyFilmsSample.xml");

        XmlDocument _xDoc = new XmlDocument();
        StreamReader _str = new StreamReader(_stream, System.Text.Encoding.UTF8);
        string _xmlStrings = string.Empty;

        while (_str.Peek() > 0)
        {
            _xmlStrings += _str.ReadLine();
        }

        File.WriteAllText(configFile, _xmlStrings, Encoding.UTF8);
        init(configFile);
    }

    public void init(string configFile)
    {
        //Chargement du fichier de conf
        XmlDocument doc = new XmlDocument();
        doc.Load(configFile);
        
        XmlNode n = doc.ChildNodes[1].FirstChild;
        XmlNodeList l = n.ChildNodes;
        
        for (int i = 0; i<l.Count; i++)
        {
            if (l.Item(i).ParentNode.Name == "Section" 
                && !l.Item(i).Name.Equals("URLSearch") && !l.Item(i).Name.Equals("Details"))
                setList(1, l.Item(i));
        }

        l = n.SelectNodes("URLSearch/*");

        for (int i = 0; i < l.Count; i++)
        {
            if (l.Item(i).ParentNode.Name == "URLSearch")
                setList(2, l.Item(i));
        }

        l = n.SelectNodes("Details/*");

        for (int i = 0; i < l.Count; i++)
        {
            if (l.Item(i).ParentNode.Name == "Details")
                setList(3, l.Item(i));
        }
    
    }

    /// <summary>
    /// Ajoute un nouvel élément dans la liste spécifiée.
    /// Paramètre list : 1 pour listGen, 2 pour listSearch, 3 pour listDetail
    /// </summary>
    public void setList(int list, XmlNode node)
    {
        XmlNode att1 = null;
        XmlNode att2 = null;

        for (int i = 0; i < node.Attributes.Count; i++)
        {
            if (i == 0)
                att1 = node.Attributes.Item(i);
            else
                att2 = node.Attributes.Item(i);
        }

        switch (list)
        {
            case 1:
                listGen.Add(new ListNode(node.Name,
                    XmlConvert.DecodeName(node.InnerText),
                    (att1 == null ? null : XmlConvert.DecodeName(att1.InnerText)),
                    (att2 == null ? null : XmlConvert.DecodeName(att2.InnerText))));
                break;
            case 2:
                listSearch.Add(new ListNode(node.Name,
                    XmlConvert.DecodeName(node.InnerText),
                    (att1 == null ? null : XmlConvert.DecodeName(att1.InnerText)),
                    (att2 == null ? null : XmlConvert.DecodeName(att2.InnerText))));
                break;
            case 3:
                listDetail.Add(new ListNode(node.Name,
                    XmlConvert.DecodeName(node.InnerText),
                    (att1 == null ? null : XmlConvert.DecodeName(att1.InnerText)),
                    (att2 == null ? null : XmlConvert.DecodeName(att2.InnerText))));
                break;
        }
    }

    /// <summary>
    /// Recherche un élément dans la liste et le retourne
    /// Paramètre list : 1 pour listGen, 2 pour listSearch, 3 pour listDetail
    /// </summary>
    public ListNode find(List<ListNode> list, string name)
    {
        return list.Find(delegate(ListNode l) { return l._Tag.Equals(name); });
    }
              
}


/*
 * Représente un noeud du fichier de conf
 * Contient le nom du tag, sa valeur, et 2 attributs.
 * 
 */ 
class ListNode
{
    private string tag = string.Empty;
    private string value = string.Empty;
    private string param1 = string.Empty;
    private string param2 = string.Empty;

    public ListNode(string tag, string value, string param1, string param2)
    {
        this.tag = tag;
        this.value = value;
        if (param1 == null)
            param1 = "";
        this.param1 = param1;
        if (param2 == null)
            param2 = "";
        this.param2 = param2;
    }

    public string _Tag
    {
        get { return tag; }
        set { tag = value; }
    }

    public string _Value
    {
        get { return this.value; }
        set { this.value = value; }
    }

    public string _Param1
    {
        get { return param1; }
        set { param1 = value; }
    }

    public string _Param2
    {
        get { return param2; }
        set { param2 = value; }
    }

}


//Class qui contient la liste des tags des fichiers de conf
//Permet d'éviter d'aller chercher dans les fichiers pour se souvenir du nom ...
// A compléter si ajout de tag.
public class TagName
{

    public static string DBName = "DBName";
    public static string URLPrefix = "URLPrefix";
    public static string URL = "URL";
    public static string KeyStartList = "KeyStartList";
    public static string KeyEndList = "KeyEndList";
    public static string KeyNextPage = "KeyNextPage";
    public static string KeyStartPage = "KeyStartPage";
    public static string KeyStepPage = "KeyStepPage";
    public static string KeyStartTitle = "KeyStartTitle";
    public static string KeyEndTitle = "KeyEndTitle";
    public static string KeyStartDirector = "KeyStartDirector";
    public static string KeyEndDirector = "KeyEndDirector";
    public static string KeyStartYear = "KeyStartYear";
    public static string KeyEndYear = "KeyEndYear";
    public static string KeyYearIndex = "KeyYearIndex";
    public static string KeyStartLink = "KeyStartLink";
    public static string KeyEndLink = "KeyEndLink";
    public static string KeyStartBody = "KeyStartBody";
    public static string KeyEndBody = "KeyEndBody";
    public static string KeyStartOTitle = "KeyStartOTitle";
    public static string KeyEndOTitle = "KeyEndOTitle";
    public static string KeyOTitleIndex = "KeyOTitleIndex";
    public static string KeyStartTTitle = "KeyStartTTitle";
    public static string KeyEndTTitle = "KeyEndTTitle";
    public static string KeyTTitleIndex = "KeyTTitleIndex"; 
    public static string KeyStartImg = "KeyStartImg";
    public static string KeyEndImg = "KeyEndImg";
    public static string KeyImgIndex = "KeyImgIndex";
    public static string KeyStartLinkImg = "KeyStartLinkImg";
    public static string KeyEndLinkImg = "KeyEndLinkImg";
    public static string KeyLinkImgIndex = "KeyLinkImgIndex";
    public static string KeyStartRate = "KeyStartRate";
    public static string KeyEndRate = "KeyEndRate";
    public static string KeyRateIndex = "KeyRateIndex";
    public static string KeyStartRate2 = "KeyStartRate2";
    public static string KeyEndRate2 = "KeyEndRate2";
    public static string KeyRate2Index = "KeyRate2Index";
    public static string KeyStartSyn = "KeyStartSyn";
    public static string KeyEndSyn = "KeyEndSyn";
    public static string KeySynIndex = "KeySynIndex";
    public static string KeyStartRealise = "KeyStartRealise";
    public static string KeyEndRealise = "KeyEndRealise";
    public static string KeyRealiseIndex = "KeyRealiseIndex";
    public static string KeyStartProduct = "KeyStartProduct";
    public static string KeyEndProduct = "KeyEndProduct";
    public static string KeyProductIndex = "KeyProductIndex";
    public static string KeyStartCredits = "KeyStartCredits";
    public static string KeyEndCredits = "KeyEndCredits";
    public static string KeyCreditsIndex = "KeyCreditsIndex";
    public static string KeyCreditsRegExp = "KeyCreditsRegExp";
    public static string KeyStartCountry = "KeyStartCountry";
    public static string KeyEndCountry = "KeyEndCountry";
    public static string KeyCountryIndex = "KeyCountryIndex";
    public static string KeyStartGenre = "KeyStartGenre";
    public static string KeyEndGenre = "KeyEndGenre";
    public static string KeyGenreIndex = "KeyGenreIndex";
    public static string BaseRating = "BaseRating";

}

