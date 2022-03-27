using System;
using System.Collections;
using System.Collections.Generic;
using Core;

namespace UI.Listeners
{
    public delegate float ObjectHeuristic(Primitive prim);

    class Objects : Listener
    {
        public int burstSize = 100;
        public float burstTime = 1;
        private DateTime burstStartTime;
        private TimeSpan burstInterval;
        public float buildingSize = 5;
        public List<Primitive> newPrims;
        public Dictionary<uint,Primitive> prims;
        private Dictionary<LLUUID, Primitive> pendingPrims;
        private Dictionary<LLUUID, Primitive> simPrims;
        private Dictionary<uint, Primitive> simObjects;
        public List<ObjectHeuristic> objectHeuristics;
        public Dictionary<LLUUID, List<Primitive>> primGroups;
        private Object newLock = new Object();
        private int maxShortNameLength = 0;
        private int maxNameLength;
        public Dictionary<string, string> shortNames;
        public Dictionary<string, string> reverseShortNames;
        public List<string> numberedObjsStr;
        public List<uint> numberedObjsID;

        public Objects(TextForm parent)
            : base(parent)
        {
            newPrims = new List<Primitive>();
            prims = new Dictionary<uint, Primitive>();
            simObjects = new Dictionary<uint, Primitive>();
            simPrims = new Dictionary<LLUUID, Primitive>();
            pendingPrims = new Dictionary<LLUUID, Primitive>();
            primGroups = new Dictionary<LLUUID, List<Primitive>>();
            shortNames = new Dictionary<string, string>();
            reverseShortNames = new Dictionary<string, string>();
            numberedObjsStr = new List<string>();
            numberedObjsID = new List<uint>();
            objectHeuristics = new List<ObjectHeuristic>();
            objectHeuristics.Add(new ObjectHeuristic(distanceHeuristic));
            objectHeuristics.Add(new ObjectHeuristic(nameLengthHeuristic));
            objectHeuristics.Add(new ObjectHeuristic(boringNamesHeuristic));

            maxNameLength = -1;

            client.Objects.OnNewPrim += new ObjectManager.NewPrimCallback(Objects_OnNewPrim);
            client.Objects.OnObjectProperties += new ObjectManager.ObjectPropertiesCallback(Objects_OnObjectProperties);

            burstStartTime = DateTime.Now;
            burstInterval = new TimeSpan(0, 0, 0, 0, (int)(burstTime * 1000));
        }

        public void ClearPrims()
        {            
            newPrims.Clear();
            prims.Clear();
            pendingPrims.Clear();
            simPrims.Clear();
            simObjects.Clear();
            primGroups.Clear();
            numberedObjsStr.Clear();
            numberedObjsID.Clear();
        }

        void Objects_OnObjectProperties(Simulator simulator, LLObject.ObjectProperties properties)
        {
            if ((pendingPrims.ContainsKey(properties.ObjectID))&&(pendingPrims[properties.ObjectID].RegionHandle == 0))
            {
                prims[pendingPrims[properties.ObjectID].LocalID] = pendingPrims[properties.ObjectID];

                //Trace.WriteLine("*****Nme:" + properties.Name + " Local:" + pendingPrims[properties.ObjectID].LocalID + " ID:" + prims[pendingPrims[properties.ObjectID].LocalID].ID + " ObjID:" + properties.ObjectID + " Parent:" + prims[pendingPrims[properties.ObjectID].LocalID].ParentID + " Pos:" + prims[pendingPrims[properties.ObjectID].LocalID].Position + "\r\n");

                //" Reg:" + prims[properties.Name].RegionHandle + "," + " Sim:" + simulator.Handle

                // object already not considered and is a parent
                if ((!simObjects.ContainsKey(pendingPrims[properties.ObjectID].LocalID)) && (pendingPrims[properties.ObjectID].ParentID == 0))
                {
                    simObjects[pendingPrims[properties.ObjectID].LocalID] = pendingPrims[properties.ObjectID];
                    if ((properties.Name.Trim().ToLower() == "object") || (properties.Name.Trim() == "") || (properties.Name == null) || (properties.Name.Length <= 2) || (properties.Name.Length > 30))
                    {
                        parent.nrofbadObjs++;
                    }
                    else
                    {
                        parent.goodObjsCumulativeLength = parent.goodObjsCumulativeLength + properties.Name.Length;
                        parent.nrofgoodObjs++;
                    }
                }
                else
                {
                    //parent.output(prims[pendingPrims[properties.ObjectID].LocalID].Data.PathScaleX + " " + prims[pendingPrims[properties.ObjectID].LocalID].Data.PathScaleY + " " +
                    //prims[pendingPrims[properties.ObjectID].LocalID].Data.PathBeginScale.X);
                }

               
                LLUUID groupId = properties.GroupID;
                if (groupId != LLUUID.Zero)
                {
                    if (!primGroups.ContainsKey(groupId))
                        primGroups[groupId] = new List<Primitive>();
                    primGroups[groupId].Add(prims[pendingPrims[properties.ObjectID].LocalID]);
                    //parent.output("group count " + groupId + " " + primGroups[groupId].Count);
                }
                pendingPrims.Remove(properties.ObjectID);
                if (maxNameLength == -1 || properties.Name.Length > maxNameLength)
                    maxNameLength = properties.Name.Length;                
            }
        }

        void Objects_OnNewPrim(Simulator simulator, Primitive prim, ulong regionHandle, ushort timeDilation)
        {
            if (prim.RegionHandle == 0)
            {
                lock (newLock)
                {
                    newPrims.Add(prim);

                    if (!simPrims.ContainsKey(prim.ID))
                    {
                        parent.nrofprims++;
                        simPrims[prim.ID] = prim;
                        if (prim.ParentID == 0)
                            parent.nrofObjects++;
                    }

                    TimeSpan dtime = DateTime.Now - burstStartTime;
                    if (newPrims.Count >= burstSize && dtime > burstInterval)
                    {
                        burstStartTime = DateTime.Now;

                        uint[] ids = new uint[burstSize];
                        for (int i = 0; i < burstSize; ++i)
                        {
                            ids[i] = newPrims[i].LocalID;
                            pendingPrims[newPrims[i].ID] = newPrims[i];
                        }

                        client.Objects.SelectObjects(simulator, ids);
                        newPrims.RemoveRange(0, burstSize);                        
                    }
                }
            }
        }

        public bool tryGetPrim(string name, out Primitive prim)
        {
            prim = null;
            int i = 0;

            string[] toks = name.Split(null);
            if (toks.Length == 2 && toks[0] == "object")
            {
                i = Convert.ToInt32(toks[1]);
                if (i > 0 && i <= numberedObjsID.Count)
                {
                    prim = prims[numberedObjsID[i - 1]];
                    return true;
                }
            }

            /*if (shortNames.ContainsKey(name))
            {

                //prim = prims[shortNames[name]];
                i = numberedObjsStr.FindIndex(delegate(string s) { return s == shortNames[name]; });
                if (i != -1)
                    prim = prims[numberedObjsID[i]];
                else
                    return false;
                return true;
            }
            else
            {*/
                IDictionaryEnumerator shortNameEnum = shortNames.GetEnumerator();
                shortNameEnum.Reset();
                //IEnumerator numobjEnum =  numberedObjsStr.GetEnumerator();
                //numobjEnum.Reset();
                string shortName = "";
                //string numName = "";
                while(shortNameEnum.MoveNext())
                {
                    shortName = shortNameEnum.Key.ToString();                    
                    if (shortName.ToUpper() == name.ToUpper())
                    {
                        //numberedObjsStr.
                        i = numberedObjsStr.FindIndex(delegate(string s) { return s.ToUpper() == shortNames[shortName].ToUpper(); });
                        if (i != -1)
                            prim = prims[numberedObjsID[i]];
                        else
                            return false;
                        return true;                        
                    }
                }
                        
            //}

            String primName = "";
            foreach (KeyValuePair<uint,Primitive> lprim in prims)
            {
                primName = lprim.Value.Properties.Name;
                if (primName.Length >= name.Length && primName.ToUpper().Substring(0, name.Length) == name.ToUpper())
                {
                    prim = lprim.Value;
                    return true;
                }
            }
            return false;
        }

        public void describePrim(Primitive prim)
        {
            parent.output(prim.Properties.Name + ": " + prim.Properties.Description);
            if (prim.Sound != LLUUID.Zero)
                parent.output("This object makes sound.");
            if (prim.Properties.SalePrice != 0)
                parent.output("This object is for sale for L" + prim.Properties.SalePrice);
        }

        public int comp(Primitive p1, Primitive p2)
        {
            return (int)(getFitness(p2) - getFitness(p1));
        }

        public List<Primitive> getPrimitives(int cap)
        {
            List<Primitive> ret = new List<Primitive>();
            foreach (Primitive prim in prims.Values)
            {
                ret.Add(prim);
            }

           /* if (ret.Count <= num)
            {
                updateNumberedObjects(ret,num);
                return ret;
            }
            else*/
            {
                ret.Sort(new Comparison<Primitive>(comp));                
                int i = 0;
                while(i<ret.Count)
                {
                    //float dist = (float)(LLVector3.Dist(client.Self.RelativePosition, ret[i].Position));
                    //parent.output(ret[i].Properties.Name + ":  " + getFitness(ret[i]) + "  " + distanceHeuristic(ret[i]) + "  " + nameLengthHeuristic(ret[i]) + "  " + boringNamesHeuristic(ret[i]) + "  " + dist + "  " + Math.Exp((double)dist) + "  " + (1 / Math.Exp((double)dist)) + "\r\n");

                    if (getFitness(ret[i]) == 0)
                        ret.RemoveRange(i, 1);
                    else
                        i++;                                        
                }
                updateNumberedObjects(ret, cap);
                if (ret.Count <= cap)
                    return ret;
                else
                    return ret.GetRange(0, cap);
            }
        }

        void updateNumberedObjects(List<Primitive> ret, int num)
        {
            numberedObjsStr.Clear();
            numberedObjsID.Clear();
            for (int i = 0; i < num && i < ret.Count; ++i)
            {
                numberedObjsStr.Add(ret[i].Properties.Name);
                numberedObjsID.Add(ret[i].LocalID);
            }
        }

        float getFitness(Primitive prim)
        {
            float fitness = 1;
            foreach (ObjectHeuristic heuristic in objectHeuristics)
            {
                fitness *= heuristic(prim);
            }
            return fitness * 100000;
        }

        float distanceHeuristic(Primitive prim)
        {
            if (prim != null)
            {      
                float dist= LLVector3.Dist(client.Self.RelativePosition, prim.Position); 
                if (dist > parent.scanRange)
                    return 0;
                else
                    return (1/(float)Math.Exp((double)dist))*3;
            }
            else
                return 0;
        }

        float nameLengthHeuristic(Primitive prim)
        {
            if ((prim != null) && (prim.Properties.Name != null))
            {
                return (float)prim.Properties.Name.Length / (float)maxNameLength;
            }
            else
                return 0;
        }

        float boringNamesHeuristic(Primitive prim)
        {
            if (prim == null)
                return 0;
            string name = prim.Properties.Name;
            if (name == "Object" || name== "object" || name == "Component" || name == null || name == " ")
                return 0;
            else
                return 1*2;
        }

        bool tryGetBuildingPos(List<Primitive> group, out LLVector3 centroid)
        {
            centroid = new LLVector3();
            if (group.Count < 4)
                return false;
            else
            {
                bool first = true;
                LLVector3 min = new LLVector3(), max = new LLVector3(), pos;
                foreach (Primitive prim in group)
                {
                    if (prim != null && prim.Position != null)
                    {
                        pos = prim.Position;

                        if (first)
                        {
                            min = pos;
                            max = pos;
                            first = false;
                        }
                        else
                        {
                            if (pos.X < min.X)
                                min.X = pos.X;
                            if (pos.Y < min.Y)
                                min.Y = pos.Y;
                            if (pos.Z < min.Z)
                                min.Z = pos.Z;

                            if (pos.X > max.X)
                                max.X = pos.X;
                            if (pos.Y > max.Y)
                                max.Y = pos.Y;
                            if (pos.Z > max.Z)
                                max.Z = pos.Z;
                        }
                    }
                }

                LLVector3 size = max - min;
                if (size.X > buildingSize && size.Y > buildingSize && size.Z > buildingSize) {
                    centroid = min + (size * (float)0.5);
                    return true;
                } else
                    return false;
            }
        }

        public int posComp(LLVector3 v1, LLVector3 v2)
        {
            return (int)(LLVector3.Mag(client.Self.RelativePosition - v1) - 
                LLVector3.Mag(client.Self.RelativePosition - v2));
        }

        public List<LLVector3> getBuildings(int num)
        {
            List<LLVector3> ret = new List<LLVector3>();
            foreach (List<Primitive> group in primGroups.Values)
            {
                LLVector3 pos = new LLVector3();
                if (tryGetBuildingPos(group, out pos))
                    ret.Add(pos);
            }

            if (ret.Count <= num)
                return ret;
            else
            {
                ret.Sort(new Comparison<LLVector3>(posComp));
                return ret.GetRange(0, num);
            }
        }

        public string getObjectName(Primitive prim)
        {
            string name = getObjectShortName(prim);
            for (int i = 0; i < numberedObjsStr.Count; ++i)
                if (numberedObjsStr[i].ToUpper() == prim.Properties.Name.ToUpper())
                    // name = (i + 1) + ": " + name;      
                    name = " " + name;
            return name;
        }

        public string getObjectShortName(Primitive prim)
        {
            if (prim.Properties.Name.Length < maxShortNameLength)
            {
                return prim.Properties.Name;
            }
            else
            {
                if (reverseShortNames.ContainsKey(prim.Properties.Name))
                    return reverseShortNames[prim.Properties.Name];
                else
                {
                    string shortName = "";
                    int i = 0;
                    foreach (string token in prim.Properties.Name.Split(null))
                    {
                        if (i != 0) shortName += " ";
                        i++;
                        shortName += token;
                        if (!shortNames.ContainsKey(shortName))
                        {
                            shortNames[shortName] = prim.Properties.Name;
                            reverseShortNames[prim.Properties.Name] = shortName;
                            return shortName;
                        }
                    }
                    return prim.Properties.Name;
                }
            }
        }
    }
}
