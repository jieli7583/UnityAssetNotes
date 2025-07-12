- put `AssetNoteWindow.cs` editor script in editor folder
- select a asset or folder in the Project tab, and write a note in the inspector tab.  
- 写完备注以后点击“Save Note”

- 在 2 by 3布局+项目单列下生效

> [!Warning]
> ### 1-column layout limitations 
> when in 2 panel mode, only folders in the right (sub-folders & assets) panel show notes on selection in the inspector.  
> when selecting a folder in the left (folder only) panel the inspector doesn't update, because unity doesn't register a selection change.  
> ![image](https://github.com/user-attachments/assets/9d3b3b2b-b0dc-484b-8208-ee0b19551b0d)  
> _left red panel, no folder notes support. right green panel, supports folder notes_
>  
> ### swap to 2-column layout 
> You can swap your Project tab to 1 panel view mode, by clicking on the vertical ...  
> ![image](https://github.com/user-attachments/assets/6bec06f7-fbe3-49a7-a756-be7000eb2337)
