/*
 * 描述：
 * 作者：Gong Chen
 * 创建时间：2018/11/29 15:44:19
 * 版本：v0.1
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SavePanel : MonoBehaviour {
    private int pageindex = 1;//1基
    private const int pageindexMax = 4;
    private const int pageindexMin = 1;
    private const int slotNumsEachPage = 8;
    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;
    public Button button5;
    public Button button6;
    public Button button7;
    public Button button8;
    public Button loadButton;
    public Button createButton;
    public Button deleteButton;
    public Text pageIndexText;

    public void pageForward()
    {
        pageindex--;
        if (pageindex < pageindexMin)
            pageindex = pageindexMin;
        reloadData();
    }

    public void pageLater()
    {
        pageindex++;
        if (pageindex > pageindexMax)
            pageindex = pageindexMax;
        reloadData();
    }

    private void reloadData()
    {
        pageIndexText.text = "第" + pageindex + "页";
        GameSave saveCondition = GameSave.getInstance();
        int slotIsSelected = saveCondition.getSlotIndexUsedAtNow();   

        SaveButton btCs1 =(SaveButton)button1.GetComponent("SaveButton");
        btCs1.setText(saveCondition.slotDescription[(pageindex - 1) * slotNumsEachPage]);
        SaveButton btCs2 = (SaveButton)button2.GetComponent("SaveButton");
        btCs2.setText(saveCondition.slotDescription[(pageindex - 1) * slotNumsEachPage + 1] );
        SaveButton btCs3 = (SaveButton)button3.GetComponent("SaveButton");
        btCs3.setText(saveCondition.slotDescription[(pageindex - 1) * slotNumsEachPage + 2] );
        SaveButton btCs4 = (SaveButton)button4.GetComponent("SaveButton");
        btCs4.setText(saveCondition.slotDescription[(pageindex - 1) * slotNumsEachPage + 3] );
        SaveButton btCs5 = (SaveButton)button5.GetComponent("SaveButton");
        btCs5.setText(saveCondition.slotDescription[(pageindex - 1) * slotNumsEachPage + 4] );
        SaveButton btCs6 = (SaveButton)button6.GetComponent("SaveButton");
        btCs6.setText(saveCondition.slotDescription[(pageindex - 1) * slotNumsEachPage + 5] );
        SaveButton btCs7 = (SaveButton)button7.GetComponent("SaveButton");
        btCs7.setText(saveCondition.slotDescription[(pageindex - 1) * slotNumsEachPage + 6] );
        SaveButton btCs8 = (SaveButton)button8.GetComponent("SaveButton");
        btCs8.setText(saveCondition.slotDescription[(pageindex - 1) * slotNumsEachPage + 7] );

        loadButton.interactable = false;
        createButton.interactable = false;
        deleteButton.interactable = false;
        /*
        if((pageindex - 1) * slotNumsEachPage<= slotIsSelected && ((pageindex - 1) * slotNumsEachPage+7)>= slotIsSelected)
        {
            //对于slotIsSelected的按钮要做图片修改（对于按钮改为被选中的图片）,这里暂时只改描述
            int buttonIndex = (slotIsSelected+1)%8;
            switch (buttonIndex)
            {
                case 0:
                    btCs8.setText("被选中");
                    break;
                case 1:
                    btCs1.setText("被选中");
                    break;
                case 2:
                    btCs2.setText("被选中");
                    break;
                case 3:
                    btCs3.setText("被选中");
                    break;
                case 4:
                    btCs4.setText("被选中");
                    break;
                case 5:
                    btCs5.setText("被选中");
                    break;
                case 6:
                    btCs6.setText("被选中");
                    break;
                case 7:
                    btCs7.setText("被选中");
                    break;
            }
        }*/
    }
    /// <summary>
    ///buttonIndex1基
    /// </summary>
    /// <param name="slotIndex"></param>
    public void slotSelect(int buttonIndex)
    {
        int slotIsSelected = (pageindex - 1) * slotNumsEachPage + (buttonIndex-1);
        GameSave saveCondition = GameSave.getInstance();
        saveCondition.setSlotIndexUsedAtNow(slotIsSelected);
        loadButton.interactable = true;
        createButton.interactable = true;
        deleteButton.interactable = true;
    }
    // Use this for initialization
    void Start () {
        pageindex = 1;
        reloadData();
        gameObject.SetActive(false);
    }

    public void createGame()
    {
        GameSave saveCondition = GameSave.getInstance();
        saveCondition.createGame();
        reloadData();
    }

    public void saveGame()
    {
        GameSave saveCondition = GameSave.getInstance();
        saveCondition.saveGame();
        reloadData();
    }

    public void deleteGame()
    {
        GameSave saveCondition = GameSave.getInstance();
        saveCondition.deleteGame();
        reloadData();
    }

    public void LoadGame_outGame(string nextSceneName)
    {
        GameSave saveCondition = GameSave.getInstance();
        if (saveCondition.presentIndexHasData())
        {
            saveCondition.save();
            SceneManager.LoadScene(nextSceneName);
        }   
    }

    public void LoadGame_inGame(string nextSceneName)
    {
        GameSave saveCondition = GameSave.getInstance();
        if (saveCondition.presentIndexHasData())
        {
            saveCondition.save();
            Destroy(TimeController.getInstance());
            World.destroyWorld();
            //地图及其它用单例模式实现的控件也需要重新加载
            SceneManager.LoadScene(nextSceneName);
        }
    }

    public void changeActive()
    {    
            gameObject.SetActive(!gameObject.activeSelf);
    }
}
