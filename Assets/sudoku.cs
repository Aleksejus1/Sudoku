using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

[System.Serializable]
public class sudoku : MonoBehaviour {
    public static int selectBlock,selectSlot,turnWithoutRemoving = 0;
    public static float distanceX,distanceY,slotDistanceX,slotDistanceY;
    internal static bool removed;
    public Sudoku puzzle;

    void Start () {
        distanceX = 170; distanceY = -170;
        slotDistanceX = 54; slotDistanceY = -54;
        puzzle = new Sudoku(GameObject.FindGameObjectWithTag("Sudoku").transform);
	}
    void Update() {
        string text = " ";
             if(Input.GetKeyDown(KeyCode.Keypad1)) text = "1";
        else if(Input.GetKeyDown(KeyCode.Keypad2)) text = "2";
        else if(Input.GetKeyDown(KeyCode.Keypad3)) text = "3";
        else if(Input.GetKeyDown(KeyCode.Keypad4)) text = "4";
        else if(Input.GetKeyDown(KeyCode.Keypad5)) text = "5";
        else if(Input.GetKeyDown(KeyCode.Keypad6)) text = "6";
        else if(Input.GetKeyDown(KeyCode.Keypad7)) text = "7";
        else if(Input.GetKeyDown(KeyCode.Keypad8)) text = "8";
        else if(Input.GetKeyDown(KeyCode.Keypad9)) text = "9";
        else if(Input.GetKeyDown(KeyCode.Backspace)) text = "";
        else if(Input.GetKeyDown(KeyCode.KeypadPeriod)) check(selectBlock,selectSlot); 
        else if(Input.GetKeyDown(KeyCode.Keypad0)) deduct();
        else if(Input.GetKeyDown(KeyCode.KeypadPlus)) Debug.Log(checkSolution());
        if (text!=" ") {
            puzzle.block[selectBlock].slot[selectSlot].text.text = text;
            if(text!="") puzzle.block[selectBlock].slot[selectSlot].removePossibilities(0,0,int.Parse(text));
            else puzzle.block[selectBlock].slot[selectSlot].addPosibilities();
        }
    }
    private void deduct() {
        removed = false;
        for(int b = 0; b < puzzle.block.Count; b++) {
            for(int s = 0; s < puzzle.block[b].slot.Count; s++) {
                if(turnWithoutRemoving<3) {
                    check(b,s);
                }
                else {
                    if(puzzle.block[b].slot[s].possibilities.Count==2) {
                        turnWithoutRemoving = 0;
                        puzzle.block[b].slot[s].remove(puzzle.block[b].slot[s].possibilities[0].value.ToString());
                        removed = true;
                    }
                }
            }
        }
        checkSingleOff();
        if(!removed) turnWithoutRemoving++;
        else turnWithoutRemoving = 0;
    }
    private bool checkSolution() {
        foreach(Block b in puzzle.block) { //Checks blocks
            int sum = 0;
            foreach(Slot s in b.slot) sum+=s.possibilities[0].value;
            if(sum!=45) return false;
        }
        for(int bline = 0; bline <= 6; bline+=3) {//Checks Horizontal Lines
            for(int sline = 0; sline <= 6; sline+=3) {
                int sum = 0;
                for(int s = sline; s < sline+3; s++) { 
                    for(int b = bline; b < bline+3; b++) {
                        sum+=puzzle.block[b].slot[s].possibilities[0].value;
                    }
                }
                if(sum!=45) return false;
            }
        }
        
        for(int bline = 0; bline < 3; bline++) {//Checks Vertical Lines
            for(int sline = 0; sline < 3; sline++) {
                int sum = 0;
                for(int s = sline; s <= sline+6; s+=3) { 
                    for(int b = bline; b <= bline+6; b+=3) {
                        sum+=puzzle.block[b].slot[s].possibilities[0].value;
                    }
                }
                if(sum!=45) return false;
            }
        }
        return true;
    }
    private void checkSingleOff() {
        //Check blocks
        for(int b = 0; b <puzzle.block.Count; b++) {
            List<int> oneOff = new List<int>();
            for(int i = 0; i < 9; i ++) oneOff.Add(0);
            for(int s = 0; s< puzzle.block[b].slot.Count;s++) {
                foreach(Possibility p in puzzle.block[b].slot[s].possibilities) {
                    oneOff[p.value-1]++;
                }
            }
            for(int o = 0; o<oneOff.Count; o++) {
                if(oneOff[o]==1) {
                    for(int s = 0; s< puzzle.block[b].slot.Count;s++) {
                        if(puzzle.block[b].slot[s].hasPossibility(o+1)) {
                            puzzle.block[b].slot[s].removePossibilities(0,0,o+1);
                            puzzle.block[b].slot[s].text.text=puzzle.block[b].slot[s].possibilities[0].value.ToString();
                            break;
                        }
                    }
                }
            }
        }
        //Check horizontally
        for(int bs = 0; bs <= 6; bs+=3) {
            for(int ss = 0; ss <=6; ss+=3) {
                List<int> oneOff = new List<int>();
                for(int i = 0; i < 9; i ++) oneOff.Add(0);
                for(int b = bs; b<bs+3; b++) {
                    for(int s = ss; s < ss+3; s++) {
                        foreach(Possibility p in puzzle.block[b].slot[s].possibilities) {
                            oneOff[p.value-1]++;
                        }
                    }
                }
                for(int o = 0; o<oneOff.Count; o++) {
                    if(oneOff[o]==1) {
                        for(int b = bs; b<bs+3; b++) {
                            for(int s = ss; s < ss+3; s++) {
                                if(puzzle.block[b].slot[s].hasPossibility(o+1)) {
                                    puzzle.block[b].slot[s].removePossibilities(0,0,o+1);
                                    puzzle.block[b].slot[s].text.text=puzzle.block[b].slot[s].possibilities[0].value.ToString();
                                    s = ss+3;
                                    b = bs+3;
                                }
                            }
                        }
                    }
                }
            }
        }
        //Check vertically
        for(int bs = 0; bs < 3; bs++) {
            for(int ss = 0; ss < 3; ss++) {
                List<int> oneOff = new List<int>();
                for(int i = 0; i < 9; i ++) oneOff.Add(0);
                for(int b = bs; b<=bs+6; b+=3) {
                    for(int s = ss; s <= ss+6; s+=3) {
                        foreach(Possibility p in puzzle.block[b].slot[s].possibilities) {
                            oneOff[p.value-1]++;
                        }
                    }
                }
                for(int o = 0; o<oneOff.Count; o++) {
                    if(oneOff[o]==1) {
                        for(int b = bs; b<=bs+6; b+=3) {
                            for(int s = ss; s <= ss+6; s+=3) {
                                if(puzzle.block[b].slot[s].hasPossibility(o+1)) {
                                    puzzle.block[b].slot[s].removePossibilities(0,0,o+1);
                                    puzzle.block[b].slot[s].text.text=puzzle.block[b].slot[s].possibilities[0].value.ToString();
                                    s = ss+7;
                                    b = bs+7;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    private void check(int b, int s) {
        if(puzzle.block[b].slot[s].text.text!="") puzzle.block[b].slot[s].removePossibilities(0,puzzle.block[b].slot[s].possibilities.Count,int.Parse(puzzle.block[b].slot[s].text.text));
        else {
            //Check current cube
            for(int sb = 0; sb< puzzle.block[b].slot.Count; sb++) {
                if(sb!=s) {
                    puzzle.block[b].slot[s].remove(puzzle.block[b].slot[sb].text.text);
                    if(puzzle.block[b].slot[s].possibilities.Count==2&&puzzle.block[b].slot[sb].possibilities.Count==2&&same(puzzle.block[b].slot[s].possibilities,puzzle.block[b].slot[sb].possibilities)) {
                        for(int bs = 0; bs < 9; bs++) {
                            if(bs!=sb&&bs!=s) {
                                puzzle.block[b].slot[bs].remove(puzzle.block[b].slot[s].possibilities[0].value.ToString());
                                puzzle.block[b].slot[bs].remove(puzzle.block[b].slot[s].possibilities[1].value.ToString());
                            }
                        }
                    }
                }
            }
            //Check Horizontal line
            int startBlock = b/3 ,startSlot = s/3;
            startBlock*=3; startSlot*=3;
            for(int sb = startBlock; sb < startBlock+3; sb++) {
            if(sb!=b) {
                for(int bs = startSlot; bs < startSlot+3; bs++) {
                    puzzle.block[b].slot[s].remove(puzzle.block[sb].slot[bs].text.text);
                    if(puzzle.block[b].slot[s].possibilities.Count==2&&puzzle.block[sb].slot[bs].possibilities.Count==2&&same(puzzle.block[b].slot[s].possibilities,puzzle.block[sb].slot[bs].possibilities)) {
                        for(int sr = startBlock; sr < startBlock+3; sr++) {
                        if(sr!=b&&sr!=sb) {
                        for(int br = startSlot; br < startSlot+3; br++) {
                            puzzle.block[sr].slot[br].remove(puzzle.block[b].slot[s].possibilities[0].value.ToString());
                            puzzle.block[sr].slot[br].remove(puzzle.block[b].slot[s].possibilities[1].value.ToString());
                        }}}
                    }
                }
            } }
            //Check Vertical line
            startBlock = b%3; startSlot = s%3;
            for(int sb = startBlock; sb <= startBlock+6; sb+=3) {
                if(sb!=b) {
                    for(int bs = startSlot; bs <= startSlot+6; bs+=3) {
                        puzzle.block[b].slot[s].remove(puzzle.block[sb].slot[bs].text.text);
                        if(puzzle.block[b].slot[s].possibilities.Count==2&&puzzle.block[sb].slot[bs].possibilities.Count==2&&same(puzzle.block[b].slot[s].possibilities,puzzle.block[sb].slot[bs].possibilities)) {
                            for(int sr = startBlock; sr <= startBlock+6; sr+=3) {
                            if(sr!=b&&sr!=sb) {
                            for(int br = startSlot; br <= startSlot+6; br+=3) {
                                puzzle.block[sr].slot[br].remove(puzzle.block[b].slot[s].possibilities[0].value.ToString());
                                puzzle.block[sr].slot[br].remove(puzzle.block[b].slot[s].possibilities[1].value.ToString());
                            }}}
                        }
                    }
                }
            }
        }
    }
    private bool same(List<Possibility> possibilities1, List<Possibility> possibilities2) {
        bool ret = true;
        foreach(Possibility p1 in possibilities1) {
            bool found = false;
            foreach(Possibility p2 in possibilities2) {
                if(p1.value==p2.value) {
                    found = true;
                    break;
                }
            }
            if(!found) {
                ret = false;
                break;
            }
        }
        return ret;
    }
}

[System.Serializable]
public class Sudoku {
    public List<Block> block;
    Transform sudokuObject;
    public Sudoku(Transform transform) {
        sudokuObject = transform;
        block = new List<Block>();
        for(int i = 0; i < 9; i++) {
            if(i<sudokuObject.childCount) {
                block.Add(new Block(sudokuObject.GetChild(i),false,i));
            }
            else {
                block.Add(new Block(sudokuObject,true,i));
            }
        }
    }
}
[System.Serializable]
public class Block {
    public List<Slot> slot;
    Transform blockObject;
    int block;
    public Block(Transform transfrom, bool createBlock, int blockID) {
        block = blockID;
        if(createBlock) {
            (blockObject = new GameObject("block").transform).SetParent(transfrom);
            blockObject.localPosition = new Vector3((((block%3)-1)*sudoku.distanceX),(((block/3)-1)*sudoku.distanceY));
        }
        else blockObject = transfrom;
        slot = new List<Slot>();
        for(int i = 0; i < 9; i++) {
            if(i<blockObject.childCount) {
                slot.Add(new Slot(blockObject.GetChild(i),false,block,i));
            }
            else {
                slot.Add(new Slot(blockObject,true,block,i));
            }
            
        }
    }
}
[System.Serializable]
public class Slot {
    public List<Possibility> possibilities;
    public Text text;
    int block,slot;
    Image image;
    Button button;
    Transform slotObject;
    public Slot(Transform transform, bool createSlot, int blockID, int slotID) {
        block = blockID; slot = slotID;
        if(createSlot) {
            (slotObject = new GameObject("slot").transform).SetParent(transform);
            slotObject.localPosition = new Vector3((((slot%3)-1)*sudoku.slotDistanceX),(((slot/3)-1)*sudoku.slotDistanceY));
            slotObject.localScale = new Vector3(0.5f,0.5f,1);
            image = slotObject.gameObject.AddComponent<Image>();
            button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(()=> {sudoku.selectBlock = block; sudoku.selectSlot = slot;});
            GameObject textObject = new GameObject("value");
            textObject.transform.SetParent(button.transform);
            text = textObject.AddComponent<Text>();
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.black;
            text.font = GameObject.FindGameObjectWithTag("Sudoku").GetComponent<Text>().font;
            text.fontSize = 32;
            textObject.transform.localPosition = Vector3.zero;
        }
        else {
            slotObject = transform;
            button = slotObject.GetComponent<Button>();
            text = slotObject.GetChild(0).GetComponent<Text>();
        }
        possibilities = new List<Possibility>();
        addPosibilities();
    }

    internal void remove(String text) {
        if(text!="") {
            for(int i = 0; i < possibilities.Count; i++) {
                try {
                    if(possibilities[i].value==int.Parse(text)) {
                        if(possibilities[i].text) GameObject.Destroy(possibilities[i].text.gameObject);
                        possibilities.RemoveAt(i);
                        sudoku.removed=true;
                        if(possibilities.Count==1) {
                            this.text.text = possibilities[0].value.ToString();
                            if(possibilities[0].text) GameObject.Destroy(possibilities[0].text.gameObject);
                        }
                        break;
                    }
                }
                catch(FormatException) { }
            }
        }
    }
    internal void removePossibilities(int startPos= 0, int count = 0, int avoidValue = 0) {
        if(count == 0) count = possibilities.Count-startPos;
        for(int i = startPos; i<startPos+count; i++) if(possibilities[i].text) GameObject.Destroy(possibilities[i].text.gameObject);
        int index = 0;
        while(possibilities.Count!=1) {
            if(possibilities[index].value==avoidValue) index++;
            possibilities.RemoveAt(index);
        }
    }

    internal void addPosibilities() {
        for(int i = 0; i < 9; i++) {
            if(possibilities.Count > i) {
                possibilities[i].value=i+1;
                possibilities[i].createText(text.transform, i);
                possibilities[i].text.text = possibilities[i].value.ToString();
            }
            else {
                Possibility pos = new Possibility(i+1);
                pos.createText(text.transform, i);
                possibilities.Add(pos);
            }
        }
    }

    internal bool hasPossibility(int o) {
        foreach(Possibility p in possibilities) {
            if(p.value==o) return true;
        }
        return false;
    }
}
[System.Serializable]
public class Possibility {
    public int value;
    public Text text;
    public Possibility(int v) {
        value = v;
    }

    internal void createText(Transform parent, int i) {
        Transform posObj = new GameObject("pos").transform;
        posObj.SetParent(parent);
        text = posObj.gameObject.AddComponent<Text>();
        text.text = value.ToString();
        text.font = GameObject.FindGameObjectWithTag("Sudoku").GetComponent<Text>().font;
        text.fontSize=8;
        text.color = Color.black;
        text.transform.localPosition = new Vector3(sudoku.slotDistanceX/6*5+((i%3)-1)*sudoku.slotDistanceX/4,sudoku.slotDistanceY/6*5+((i/3)-1)*sudoku.slotDistanceY/4);
    }
}