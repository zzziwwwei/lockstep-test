# 幀同步實作(含預測、回滾)

> * 連線遊戲為追求本地響應速度使用回滾預測機制。
> * 回滾機制對網路延遲較高者體驗很差，對延遲較低者能提升更多操作空間。
> * 回滾動態太高調整輸入延遲(InputDelay)延遲遊戲執行時間緩衝網路延遲。

##
##
## 遊戲預覽(預設延遲)
![image](https://github.com/zzziwwwei/androidGame/blob/main/readme/%E9%80%A3%E7%B7%9A.gif)
##
## 架構圖
![image](https://github.com/zzziwwwei/androidGame/blob/main/readme/%E5%B9%80%E5%90%8C%E6%AD%A5%E5%AF%A6%E4%BD%9C%E6%9E%B6%E6%A7%8B%E5%9C%96.jpg)

##
> * Input:處理本地輸入與其他client端輸入
> *  Websocket:使用websocket作為連線插件處理傳輸封包與接收封包
> * GameLog(inputLog):紀錄遊戲過程，照著Log執行遊戲
> * Predict:尚未接收到訊號已pre-log代替，事先預測進度
> * Rollback:接收訊號後若與預測不符合，執行回滾
> * InGame:遊戲事件，腳色管理
##
##
> *[使用連線插件](https://github.com/psygames/UnityWebSocket)




