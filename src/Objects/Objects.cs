using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Xml.Serialization;

namespace Objects
{
    public enum messageType
    {
        playerLogin, error, gameMasterLogin, loginResponse, 
        beginGame, gameState, move, thankYou, logout,serverShutdown,
        championsList,playerLeftGame, leaveGame
    }
    
    public enum accept
    {
        yes, no
    }

    public enum result
    {
        winner,looser
    }
    
    public class error
    {
        //tag present only when accept="no" Error ids: 1 - wrong nick 2 - improper game type 3 - players pool overflow 
        //4 - master for this game already registered 5 - wrong game type description data
        [XmlAttribute("id")]
        public int _id;
    }

    public class playerLogin
    {
        [XmlAttribute("nick")]
        public string _nick;
        [XmlAttribute("gameType")]
        public string _gameType;
    }

    public class gameMasterLogin
    {
        [XmlAttribute("id")]
        public string _id;
        [XmlAttribute("gameType")]
        public string _gameType;
        [XmlAttribute("playersMin")]
        public int _playersMin;
        [XmlAttribute("playersMax")]
        public int _playersMax;
    }

    public class loginResponse
    {
        [XmlAttribute("accept")]
        public accept _accept;
        [XmlElement("error")]
        public error _error;
    }

    public class gameId
    {
        [XmlAttribute("id")]
        public string _id;
    }

    public class player
    {
        [XmlAttribute("nick")]
        public string _nick;
        [XmlAttribute("result")]
        public result _result;
        [XmlAttribute("lost")]
        public int _lost;
        [XmlAttribute("won")]
        public int _won;
    }
    
    public class nextPlayer
    {
        [XmlAttribute("nick")]
        public string _nick;
    }
    
    public class point
    {
        [XmlAttribute("x")] 
        public int _x;
        [XmlAttribute("y")]
        public int _y;
    }
    public class gameState
    {
        [XmlElement("point")]
        public List<point> _point;
    }
    /// <summary>
    /// Object of all possible message types
    /// </summary>
    public class message
    {
        [XmlText]
        public string _text;
        [XmlAttribute("type")]
        public messageType _type;
        [XmlElement("playerLogin")]
        public playerLogin _playerLogin;
        [XmlElement("gameMasterLogin")]
        public gameMasterLogin _gameMasterLogin;
        [XmlElement("response")]
        public loginResponse _loginResponse;
        [XmlElement("gameId")]
        public gameId _gameId;
        [XmlElement("nextPlayer")]
        public nextPlayer _nextPlayer;
        [XmlElement("move")] 
        public List<point> _point;
        [XmlArray("gameOver")]
        public List<player> _gameOver;
        [XmlElement("player")] 
        public List<player> _players;
        [XmlElement("gameState")]
        public gameState _gameState;
    }

}
