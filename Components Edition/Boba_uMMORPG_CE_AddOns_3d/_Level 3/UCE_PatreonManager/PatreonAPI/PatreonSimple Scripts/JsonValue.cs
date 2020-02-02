/*
	Written by: guzuligo@gmail.com
	Licence: https://opensource.org/licenses/MIT
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class JsonValue  {

    
    static int pt=-1;//parse at
    static char[] toParse;
	public string value;

    public JsonValue parent=null;
	public Dictionary<string,JsonValue> map;
    public JsonValue this [string ind_]{
            get{
                if (map.ContainsKey(ind_))
                    return map[ind_];
                    else
                        return new JsonValue();  
            }//get
    }
    public bool has(params string [] path_){
        Dictionary<string,JsonValue> m=map;
        for (int i=0;i<path_.Length;i++){
            //Debug.Log(path_[i]+":"+(m!=null));
            if (m==null || !m.ContainsKey(path_[i]))
                return false;
            m=m[path_[i]].map;
        }
        return true;
    }

    public string get(params string [] path_){
        JsonValue m=this;
        for (int i=0;i<path_.Length;i++){
            //Debug.Log(path_[i]+":"+(m!=null));
            if (!m.map.ContainsKey(path_[i]))
                return null;
            m=m.map[path_[i]];
        }
        return m.value;
    }

    public char valueType='v';//v:value o:object a:array
    public JsonValue parse(string txt=""){
        if (txt!=""){
            value=txt;
            toParse=txt.ToCharArray();
            pt=-1;
        }
        
        string key="";
        while(pt<toParse.Length){
            //STEP 1:look for key 
            switch(toParse[++pt]){
                case '{'://initialize object
                    valueType='o';
                    map=new Dictionary<string, JsonValue>();
                    continue;
                //break;
                case '['://initialize array
                    valueType='a';
                    map=new Dictionary<string, JsonValue>();
                    //TODO: exit this procedure and do array lookup
                break;

                //termination
                //TODO:terminate object
                case '}':return this;//break;
                //TODO:terminate array
                case ']':return this;//break;
                //TODO: next item
                case ',':
                    if (valueType=='o'){
                         key="";
                         continue;
                }
                break;


                //skip empty
                case ' ': case '\n':case '\t':continue;


                // (")
                case '"':
                   key="";
                    //get key name
                    while(toParse[++pt]!='"')key+=toParse[pt];
                    //skip till colon
                    while(toParse[++pt]!=':');
                break;
                // (")
                case '\'':
                   key="";
                    //get key name
                    while(toParse[++pt]!='\'')key+=toParse[pt];
                    //skip till colon
                    while(toParse[++pt]!=':');
                break;
                

                //keys that has no (")
                default:
                   key="";
                   pt--;
                   //Debug.Log("tt:"+toParse[pt]);
                   while(toParse[++pt]!=':')
                    if (toParse[pt]!=' ' && toParse[pt]!='\n' && toParse[pt]!='\t')
                        key+=toParse[pt];
                break;
            }
            //Debug.Log("Key>"+key);

            //if no key found yet, keep looking
            if (key=="" && valueType!='a')
                continue;
            //prepare for array
            if (valueType=='a'){
                if (key=="")
                    key="0";
                else
                    key=(int.Parse(key)+1).ToString();
            }
            //STEP 2: look for value,array,object
            
            //skip empty area
            do{pt++;}
            while(toParse[pt]==' '  || toParse[pt]=='\n'
                                    || toParse[pt]=='\t');
            //pt--;
            //Debug.Log("value is: "+toParse[pt]);
            switch(toParse[pt]){
                case '{':case '[':
                    pt--;
                    map[key]=new JsonValue().parse();
                break;
                case '"': 
                    map[key]=new JsonValue();
                    map[key].value="";
                    while(toParse[++pt]!='"')
                        map[key].value+=toParse[pt];
                break;

                case '\'':
                    map[key]=new JsonValue();
                    map[key].value="";
                    while(toParse[++pt]!='\'')
                        map[key].value+=toParse[pt];
                break;
                default:
                    map[key]=new JsonValue();
                    map[key].value="";
                    pt--;
                    while(      toParse[++pt]!=' '
                        &&      toParse[pt]!='\n'
                        &&      toParse[pt]!='\t'
                        &&      toParse[pt]!=','
                        &&      toParse[pt]!=']'
                        &&      toParse[pt]!='}')
                            map[key].value+=toParse[pt];
                    //
                    pt--;//next iteration needs to know it

                continue;//break;
            }
        }

        return this;
    }

}
