using UnityEngine;
using System.Collections;


public class JItemOption {
	public int r;
	public int g;
	public int b;
}

public class PacketBundle
{
	public class config_ {
		public int idx;
		public string val;
	}
	
	// test classes
	public class post_ {
		public int testA;
		public int testB;
		//public int seq;
	}
	
	public class input_ {
		public post_ post;
	}
	//
	
	public class login_ {
		public string sid;
		public int cid;
		public string svr;
		public string ver;
		public int tic;
		
		//public string Ver{ get{ return ""+ver;}}
	}
	
	
	//account informations
	public class accLabel_ {
		public int cid;
		public string name;
		public int level;
	}
	
	public class account_ : accLabel_ {
		public int gender;
		public int exp;
		public int gold;
		public int cash;
		public int acp; //active point
		public int pap; //pasive point
		public int cst;
		public int mst;
		
		public int etype;
		public int clss;		
	}
	
	public class myAccount_ : account_ {
		public int stick;
		public int flevel;
		public int fexp;
		
		//village friend attributes
		public int n1type;
		public int n1lvl;
		public int n2type;
		public int n2lvl;
		public int rtick;
		
		//dungeon friend attributes
		public int atk;
		public int defp;
		public int hp;
		public int sp;
		public int ptick;
	}
	
	public class friendAccount_ : account_ {
		//village friend attributes
		public int n1type;
		public int n1lvl;
		public int n2type;
		public int n2lvl;
		public int rtick;
		
		//dungeon friend attributes
		public int atk;
		public int defp;
		public int hp;
		public int sp;
		public int ptick;
	}
	//
	
	public class skill_ {
		public int id;
		public int lv;
	}
	
	public class item_ {
		public int uid;
		public int iid;
		public string opt;
		public int cnt;
	}
	
	
	//stage structures
	public class DropItem {
		public int exp;
		public int gold;
		public int[] items;
	}
	
	public class ObjectReward {
		public int idx;
		public int umid; //unique monster id
		public int iid;
		
		public DropItem drop;
	}
	
	public class stageinfo_ {
		public int suid;
		public ObjectReward[] sglist;
	}
	
	public class rewards_ {
		public int exp;
		public int gold;
		public item_[] items;
	}
	
	public class stageresult_ {
		public rewards_ rewards;
	}

	public class Error_
	{
		public int id;
		public int lv;
		public string msg = string.Empty;

		public string ErrorMessage { get { return string.Format( "{0}, id : {1}, lv : {2}", msg, id, lv ); } }
	}
	//
	
	public config_[] config;

	public post_ post;
	public input_ input;
	
	public login_ login;
	public myAccount_ info;
	public item_[] equip;
	public skill_[] skill;
	
	public friendAccount_[] flist;
	public accLabel_[] fwlist;
	
	public stageinfo_ stageinfo;
	public stageresult_ stageresult;

	public Error_ err;
}
