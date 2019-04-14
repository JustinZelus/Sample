package icm.com.tw.vcusb;
//Truck
//Update 2.2.8
//Date 2017-03-01
//1. 2.2.7問題依然存在，直接使用JavaExcel Library JXL.jar，匯出UTF-8版本的xls檔案

//Update 2.2.7
//Date 2017-03-01
//1. CSV有UTF-8亂碼問題，新增Excel輸出函數
//exportExcel()
//updateAndExportExcel(String tableName)
//writeExcel(final File filePath,final String outputStr,final String strEncoding)

//Update 2.2.6
//Date 2017-02-24
//1. 新增寫完資料庫後，PC也可立即顯示
//2. 新增資料庫db轉CSV函數
//3. 修改DBHelper.java 變數繼承修改

//Update 2.2.5
//Date 2016-11-25
//Truck
//Update 2.2.4
//Date 2016-11-25
//1.新增table改vss數值的類別,Gauges使用,用來把有誤的數值做掉
//2.變數orange相關要改成yellow,變數white要改成orange(done)
//注意:
//白色改成橘色,程式碼沒改,只改底層圖片,待日後有空一並修改(done)
//colorDiaologFragment架構不好,改程式碼找很久


//Truck
//Update 2.2.3
//Date   2016-11-21
//1. 加入JUNCHU車種，可藉由外部檔案 sdcard根目錄 truck_vss_values.txt，直接修改錶的對應數據

//Update 2.2.2
//Date   2016-11-17
//1. 加入JUNCHU車種，VSS數值變化客製化 (GaugeView & SquareView)

//Update 2.2.1
//Date   2016-11-15
//1. 修正當無LV DATA ID時，自動填充ID 0xFFFF，避免沒有資料，造成循環INIT的問題
//2. 增加當IO Exception LOG增加換行

//Update 2.1.8
//Date 2016-10-24
//1.新增類別 MainViewFragmentModel, ColorDialogFragment : 
//兩個類有交集 , 其功能為 "長按"然後選你喜歡的顏色,下次再開啟app時會載入上次所選的顏色
//如果要測試,在onResume 有thread

//Update 2.1.7
//Date 2016-10-03
//1.新增screenshot之後立即在pc電腦上更新檔案的功能 (mac需要重新拔插usb)

//Update 2.1.6
//Date 2016-08-29
//1.修正igOff 5秒時間還未到就重開問題


//Update 2.1.5
//Date 2016-08-05
//1.修正在6.0版本,gauge出現的銳角問題

//Update 2.1.4
//Date 2016-07-22
//1.修正駕駛日期顯示2016/04/22 bug

//Update 2.1.3
//Date 2016-06-23
//1.修改VSS倍率

//Update 2.1.0
//Date 2016-06-22
//1.新增screenshot功能 及其相關檔案


//Update 2.1.0
//Date 2016-06-13
//1. 原vid 輸入dialog 改成tvNE0 dialog,NE為protocol新增的值,在settingTable有新增欄位"PP"
//2. NG1,NG2會根據NE的值做計算,
//3. NE輸入的值會紀錄在介面,下次重開app會顯示
//4. NE在settingTable為pp, 在resultTable為NE
//5. resultTable中原來的accelOFF 改為 燃料無噴射走行比率 (但沒有增加resultTable資料庫欄位,是用原本的欄位修改值)



//Update 2.0.9
//Date 2016-05-26
//1. 新增IdleStopwatch.class ，內含一個計時的計時器與一個總合計時計，
//並內建開始時間、結束時間、累計時間與計時時間並計算總PERCENT.
//2. 並修改卡車GaugesFragment與ResultsFragment各項時間UI，以配合IdleStopwatch計算時間

// Multiple Mode
//Update 2.0.8
//Date 2016-04-27 Wed
//1. GaugeView 使用樓下試車時的數據舊算法，RpmGaugeView使用新的算法
//	 Optional Mode的四個小錶 父類別AbstractGauge也使用新算法,
//	 目前改為舊算法
//2. GaugeView VSS倍率從1.10改為1.05

//Update 2.0.7
//Date 2016-04-22 Fri
//1. ResultsFragment，靜態瀏覽功能添加讀取資料庫歷史紀錄功能
//Date 2016-04-25 Mon
//1. 拿掉truck_asus_1280_800_main_fragment.xml layout的喇叭按鈕
//2. 更新truck_asus_1280_800_option_fragment.xml layout的兩張圖
//	 option1.png , option2.png , option1_pressed.png , option2_pressed.png

//Update 2.0.6
//Date 2016-04-21 Thu
//1. Beep Mp3 檔案更新
//2. Option Mode 第一次直接點擊開始，再點擊停止，進入結果頁面，程式Crash問題
//3. Layout改回8-in 1280x800
//4. 修正新增4種聲音後，setting選項選擇0分鐘，警告仍然會響的問題

//Date 2016-04-22 Fri
//1. 修改MP3Player setAssets()函數，將每次重新release() 重新new，改為reset 再重新設定，
//	 如此一來，程式不會一直重新new MediaPlayer物件

//Update 2.0.5
//Date 2016-04-20 Wed
//1. MP3Player更新，新增不同警告燈，發出不同響聲
//2. 更新Optional Mode 4個Gauge，用以符合7-in 1024x600
//	 sedan_4th_asus_1024_600_monitor_main_fragment.xml layout

//Update 2.0.4
//Date 2016-04-18 Mon
//1. 加入AppMode區分
//2. 加入CarType區分
//3. GlowingText class 歸類到icm.com.tw.util.ui package

//Date 2016-04-19 Tue
//1. 加入DBHelper.

//Update 2.0.3
//Date 2016-04-13 Wed
//1. 更新TimerCaluculator.java
//2. 更新CrashHandler.java
//3. 更新ResultFragment.java (支援 HOUR)

//變更PackageName :
//icm.com.tw.vcusb.truck.fragment ->
//icm.com.tw.vcusb.fragment.truck
//新增兩個PackageName :
//icm.com.tw.vcusb.fragment.sedan
//icm.com.tw.vcusb.ui.sedan

//Update 2.0.2
//Date 2016-04-12 Tue
//1. 新增Results_Fragment.java 動態觀看結果功能
//2. package name更改:
//package icm.com.tw.vcusb.truck;
//變更為
//package icm.com.tw.vcusb.ui.truck;

//Update 2.0.1
//Date 2016-04-07 Thu
//1. BZC設定為0時，按下運轉按鈕，綠色警報圖式瞬閃問題 
//2. 修改unpackVdiDatas()函數，遇到0x00026A6C時(IgOff)，返回true

//Date 2016-04-08 Fri
//1. 修改unpackDatas()，遇到0x0002FF01，返回true

//Update 2016-04-11 Mon
//1. 新增CommUsbModule.java , DataUnpacker.java(MainActivity未移植)
//2. CommandFactory.java 新增 public byte[] createFormattedCommand(byte[]cmd) 函數

//Update 2.0.0
//Date 2016-04-01 Fri
//1. STANDARD & OPTIONAL MODE 2 IN 1
//2. 添加MONITOR_MAIN & MAIN FRAGMENT可以返回選擇頁面

//Date 2016-04-06 Wed
//1. 修改通訊端，先解完資料再繼續下一輪 ，如果收回來的資料錯誤，就繼續重送。


//Single Mode
//Product: Truck
//Update 1.5.1
//Date 2016-04-13 Wed
//1. 拿掉喇叭
//2. 解決off之後還左右燈還持續亮的問題
//3. backpressed 返回鍵設定無效化，並用一個 isMultipleMode 旗標控制

//Update 1.5.0
//Date 2016-04-01 Fri
//1. 改成 7 inch的layout 

//Update 1.3.0
//Date 2016-04-01 Fri
//1. 修改unpackDatas()函數，遇到0xFF 將重新Init旗標設置為true ;
//2. 修改sendAndReadData()要收齊至少4個bytes，才會開始截取封包長度，
//	   新增收齊資料後，顯示花費時間
//3. 每個輪詢的通訊間隔時間設為1ms

//Date 2016/03/31 Thu
//1. 整合STANDARD & OPTIONAL FRAGMENT
//2. 新增unpackDatas函數，直接判斷目前資料為哪個型態(LV,INFO,FREEZE)就去解哪一段
//3. tempBuffer複製問題 ，將 FT311UARTInterface.java 的ReadData()函數加上同步化
//		03-31 09:03:24.422: D/VC_TRUCK_USB(14026): i = 0, Send Datas : 28 bytes , 001A2A0001000200030004000600070009000A006400650066006704
//		03-31 09:03:24.440: D/VC_TRUCK_USB(14026): i = 0 , Recv Datas : 25 bytes, 0000000000000044D480003F80000000000000000000000000
//		03-31 09:03:24.440: D/VC_TRUCK_USB(14026): i = 0, Total Recv Datas : 25 bytes , 0000000000000044D480003F80000000000000000000000000
//		03-31 09:03:24.440: D/VC_TRUCK_USB(14026): OVER RECV LENGTH : 25 
//4. VSS表要餵數值之前先乘1.10
//5. 新增SettingFragment 的初期設定功能
//6. 修復設定值MD5相同時，不再寫入EEROM
//7. unpackDatas()函數遇到 0x0002FF01 時，設定isAlreadyInit旗標為false
//8. 修改option_fragment layout 的mode文字, One -> Standard Mode , Two -> Optional Mode

//Update 1.2.9
//Date 2016/03/29 Tue
//1. 修改SettingTable讀取預設值
//2. 修復按下設定按鈕崩潰部分(通訊)
//	  實際上試車會遇到一開始送0xFC，導致指令初始化有問題，加上傳送和保護機制
//3. App一開始為讀取MainActivity.SettingValue的數據，改為直接讀取SettingTable的設定值
//4. 按下開始鈕之後，警報聲和按鈕可取消
//5. 通訊送收 Timer 100ms 修改  System.currentTimeMillis() 為Stopwatch()  
//6. 修改 按下設定畫面時，每次都會去通訊問當前設定值
//7. Setting KF3 修復

//Update 1.2.8
//Date 2016/03/28 Mon
//1. 修改SettingTable讀取預設值
//2. 修復按下設定按鈕崩潰部分(UI)
//3. 新增CrashHandler 類別，監控意外發生錯誤時，紀錄log到檔案

//Update 1.2.7
//Date 2016/03/28 Mon
//1. 修改  當快速重覆發動引擎時，App遇到 0x0002FF01時，沒有Initial的問題

//Update 1.2.6
//Date 2016/03/26 Sat
//1. 修改各個unpackXXXFunction() ， 加入防護機制，每次接收byte長度最少為4，
//	 response cmd 如果指令不對，直接返回false
//2. 修改SendAndRecvData()函數，原判斷長度收到兩個bytes便進入長度運算返回，改為至少要四個bytes.

//Update 1.2.5
//Date 2016/03/25 Fri
//1. NEC平板當IG OFF有時會出現 "平板APP無法關閉現象"，因此在FT311UARTInterface.java中
//	   新增一個pipeErrorTimes變數與getPipeErrorCount()函數，並修改MainActivity.java，在狀態機中
//	   加入監控pipeErrorTimes變數，當數據大於0以上，如果App還在運行，直接將程式關閉。
//2. 避免上述情形發生，按下START且IG OFF後，會發生來不及存檔直接執行CloseApp()，因此在MonitorState
//	   新增一旗標isSaveOkInIgOff，當此旗標為TRUE時，且getPipeErrorCount()>100時，才關閉程式。
//3. 新增遇到0x0002FF01 ERROR 判斷，重新Initial
//4. 修復FREEZE DATA 0x3101 與 0x3102互調問題
//5. 資料庫修改 UserTable增加android_sn欄位
//	 ResultTable增加result_total_time欄位

//Update 1.2.4
//Date 2016/03/24 Thu
//1. 修改BUZZER BTN , BZ , BZC 邏輯增加

//Update 1.2.3
//Date 2016/03/24 Thu
//1. 修改BUZZER SWITCH STATE CHANGE
//2. IG OFF時，加入數據儲存功能

//Update 1.2.2
//Date 2016/03/23 Wed
//1. 增加喇叭音效
//2. 修改BUZZER OFF為按鈕型態
//3. 結果顯示頁面TEXT VIEW增長，字體靠右

//Update 1.2.1
//Date 2016/03/22 Tue
//1. 修改資料庫 NaN , Infinity問題
//2. 整合最後頁面資訊至資料庫

//Update 1.2.0
//Date 2016/03/18 Fri

//Update 1.1.1
//Date 2016/02/26 Fri
//1. 修改Fragment切換

//Update 1.1.0
//Date 2016/02/25 Thur
//1. UI框架更改為Fragment
//2. warningDialog 加入返回按鍵無效化
//3. exitDialog 加入返回按鍵無效化
//4. MainActivity 加入返回按鍵無效化
//5. 更新Activity為AppCompatActivity，新增ActionBar

//Update 1.0.2
//Date 2016/02/24 Wed
//1. 修改LANDSCAPE 為 REVERSE_LANDSCAPE

// Update 1.0.1
// Date 2016/02/23 Tue
// 1. 方向燈不可按
// 2. 取消Exit鈕
// 3. 方向燈閃爍慢Timer

/**
 * Created by K.C. on 2016/2/16.
 */

import android.annotation.SuppressLint;
import android.annotation.TargetApi;
import android.app.Activity;
import android.app.AlertDialog;
import android.app.Fragment;
import android.app.FragmentManager;
import android.app.FragmentTransaction;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.SharedPreferences;
import android.database.Cursor;
import android.graphics.drawable.Drawable;
import android.os.Build;
import android.os.Bundle;
import android.os.Environment;
import android.os.Handler;
import android.os.Message;
import android.util.Log;
import android.view.KeyEvent;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.WindowManager;
import android.widget.Button;
import android.widget.CompoundButton;
import android.widget.ListView;
import android.widget.SimpleAdapter;
import android.widget.TextView;
import android.widget.Toast;
import android.widget.ToggleButton;

import java.io.BufferedOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Random;

import icm.com.tw.communication.MonitorStates;
import icm.com.tw.communication.VdiCommand;
import icm.com.tw.communication.VdiConstants;
import icm.com.tw.util.BytesTool;
import icm.com.tw.util.CrashHandler;
import icm.com.tw.util.CustomLocale;
import icm.com.tw.util.IdleStopwatch;
import icm.com.tw.util.LogManager;
import icm.com.tw.util.MP3Player;
import icm.com.tw.util.ProgramInfo;
import icm.com.tw.util.SingleMediaScanner;
import icm.com.tw.util.Stopwatch;
import icm.com.tw.util.StopwatchFull;
import icm.com.tw.util.sqlite.DBHelper;
import icm.com.tw.util.sqlite.SedanDBHelper;
import icm.com.tw.util.sqlite.TruckDBHelper;
import icm.com.tw.util.sqlite.values.SettingValue;
import icm.com.tw.vctruckusb.R;
import icm.com.tw.vcusb.AppAttributes.AppMode;
import icm.com.tw.vcusb.AppAttributes.CarType;
import icm.com.tw.vcusb.AppAttributes.TabletType;
import icm.com.tw.vcusb.fragment.sedan.MainviewFragment_4th;
import icm.com.tw.vcusb.fragment.truck.GaugesFragment;
import icm.com.tw.vcusb.fragment.truck.LogoFragment;
import icm.com.tw.vcusb.fragment.truck.MainviewFragment;
import icm.com.tw.vcusb.fragment.truck.OptionFragment;
import icm.com.tw.vcusb.fragment.truck.Page2Fragment;
import icm.com.tw.vcusb.fragment.truck.ResultsFragment;
import icm.com.tw.vcusb.fragment.truck.ResultsFragment.ShowMode;
import icm.com.tw.vcusb.fragment.truck.SettingTable;
import icm.com.tw.vcusb.fragment.truck.SettingTableFragment;
import icm.com.tw.vcusb.ui.truck.GaugeView;
import icm.com.tw.vcusb.ui.truck.RPMGaugeView;
import icm.com.tw.vcusb.ui.truck.SquareView;

//import android.app.Fragment;
//import android.app.FragmentManager;
//import android.app.FragmentTransaction;

//class App extends Application{
//    @Override 
//    public void onCreate() {  
//        super.onCreate();  
//        CrashHandler crashHandler = CrashHandler.getInstance();  
//        //注册crashHandler  
//        crashHandler.init(getApplicationContext());  
//    }  
//}

//public class MainActivity extends AppCompatActivity
public class MainActivity extends Activity {
    // enum CarType
    // {
    // TRUCK,
    // PASSENGER_CAR
    // }

    private static boolean isNeedResend = true;
    private boolean isIdle = false;

    //儲存送收LOG用的陣列
    private ArrayList<String> sendRecvLogContents = new ArrayList<String>();
    //儲存送收LOG的最大筆數
    private final int SEND_RECV_LOG_MAX_COUNT = 1000;

    public enum PageName {
        Logo, Main, Setting, Monitor_Main, Result, Option, MAIN_FRAGMENT_4TH, RESULT_FRAGMENT_4TH;
    }


    private boolean isMultipleMode = false;


    // Page Value
    public static PageName CurrentPage = PageName.Logo;

    //SedanFragment
    public static MainviewFragment_4th mainviewFragment_4th;

    // TruckFragment
    public static OptionFragment optionFragment;
    public static LogoFragment logoFragment;
    public static SettingTableFragment settingTableFragment;
    public static MainviewFragment mainview_fragment;
    public static GaugesFragment gauges_fragment;
    public static ResultsFragment results_fragment;
    public static Page2Fragment p2_fragment;
    // public static DebugFragment debug_fragment;

    private static MP3Player mp3Player = null;
    private static SettingValue settingValue = new SettingValue();
    public static String androidSerialNumber = "";

    private final long sendPeriodMillisec = 1;

    private static DBHelper dbHelper;
    private static String dbStoreDir = null;
    private final static String truckDirName = "VcTruck";
    private final static String sedanDirName = "VcPassengerCar";
    private final static int _DBVersion = 1; // <-- 版本
    private final static String _TruckDBName = "VcTruck.db"; // <-- db name
    private final static String _SedanDBName = "VcPassengerCar.db"; // <-- db name


    private static String dirName = "";
    private static String _DBName = "";

    // final CarType carType = CarType.PASSENGER_CAR;

    public static Activity activity = null;
    private static boolean commOK = true;

    // 方向燈閃爍時間
    private final long blinkingTimeMilliSec = 333;

    // fragment list
    private List<Fragment> fragmentList = new ArrayList<Fragment>();

    final int TRUCK_INDICATOR_ON = 1;
    final int TRUCK_INDICATOR_OFF = 0;
    final int SEDAN_INDICATOR_ON = 0;
    final int SEDAN_INDICATOR_OFF = 1;

    int INDICATOR_ON;
    int INDICATOR_OFF;
    // final int INDICATOR_ON = TRUCK_INDICATOR_ON;
    // final int INDICATOR_OFF = TRUCK_INDICATOR_OFF;
    // final int INDICATOR_ON = SEDAN_INDICATOR_ON;
    // final int INDICATOR_OFF = SEDAN_INDICATOR_OFF;

    final int UPDATE_READ_DATA = 1;
    final int UPDATE_READ_DATA_DONE = 4;
    final int UPDATE_SEND_DATA = 2;
    final int UPDATE_SEND_DATA_DONE = 3;

    // 物件鎖
    private final static Object LOCKER = new Object();
    private final static Object ACT_HORN_ACTIVE_LOCKER = new Object();
    private final static Object ACT_INDICATOR_ACTIVE_LOCKER = new Object();
    private final static Object RECONNECT_LOCKER = new Object();

    // 訊息字串
    final String WARNING_TITLE = "Warning";
    final String WARNING_MESSAGE = "Can't detect device or get USB permission.\r\nPress Okay to IG Off and On.";
    final String MSG_OK = "OK";
    final String Exit_Title = "Exit App ?";
    final String Exit_MESSAGE = "Press \"YES\" to Exit App.\r\nPress \"NO\" to continue.";
    final String MSG_YES = "YES";
    final String MSG_NO = "NO";

    // ACT 作動旗標
    private static boolean IsHornAct = false;
    private static boolean IsLeftIndicatorAct = false;
    private static boolean IsRightIndicatorAct = false;
    private static boolean IsLeftIndicatorButtonOn = false;
    private static boolean IsRightIndicatorButtonOn = false;
    private static boolean IsLeftIndicatorCloseOK = false;
    private static boolean IsRightIndicatorCloseOK = false;
    private static boolean IsRoomLampCloseOK = false;

    // //計時器
    // static Stopwatch loopSw = Stopwatch.createStarted();
    // static Stopwatch sendAndRecvSw = Stopwatch.createStarted();
    // static Stopwatch actSw = Stopwatch.createStarted();
    // static Stopwatch socketRebuildSw = Stopwatch.createStarted();
    // static Stopwatch errSw = Stopwatch.createStarted();
    // static Stopwatch indicatorLeftTimer = Stopwatch.createStarted();
    // static Stopwatch indicatorRightTimer = Stopwatch.createStarted();
    // static Stopwatch indicatorAllTimer = Stopwatch.createStarted();
    // static Stopwatch reconnectTimer = Stopwatch.createStarted();

    static Stopwatch indicatorLeftTimer = new Stopwatch();
    static Stopwatch indicatorRightTimer = new Stopwatch();
    static Stopwatch indicatorAllTimer = new Stopwatch();
    //	public static Stopwatch idleTimer = new Stopwatch();
    public static IdleStopwatch newIdleTimer = new IdleStopwatch();


    public static long idleTotoalTime = 0;
    public static long newIdleTotoalTime = 0;
    public static long tempTime = 0;

    private StopwatchFull igOffTimer = new StopwatchFull();
    public static final String CLASSNAME = "VC_USB_TRUCK";
    String AppName = "VC USB Truck";

    public static boolean isAlreadyInit = false;
    private static boolean isAlreadyGetLvCmd = false;

    /* declare a FT311 UART interface variable */
    public FT311UARTInterface uartInterface;

    // Button configButton,savefileButton;
    Button sendfileButton1;
    Button sendactcmdButton;
    Button pauseButton;
    Button exitButton;

    /* local variables */
    // byte[] writeBuffer;
    static byte[] readBuffer;
    int[] actualNumBytes;

    byte numBytes;
    byte count;
    byte status;
    byte writeIndex = 0;
    byte readIndex = 0;

    int baudRate; /* baud rate */
    byte stopBit; /* 1:1stop bits, 2:2 stop bits */
    byte dataBit; /* 8:8bit, 7: 7bit */
    byte parity; /* 0: none, 1: odd, 2: even, 3: mark, 4: space */
    byte flowControl; /* 0:none, 1: flow control(CTS,RTS) */

    private static final String FILE_NAME = "SavedFile.txt";
    private static final String ACCESS_FILE = android.os.Environment.getExternalStorageDirectory()
            + java.io.File.separator + FILE_NAME;

    private FileInputStream fos_open;
    private FileOutputStream fos_save;
    private BufferedOutputStream buf_save;

    ArrayList<Thread> threadList = new ArrayList<Thread>();

    // App狀態
    public static boolean isAppExit = false;
    public static boolean isAppPause = false;

    // ECU各項狀態
    private static boolean bCanUsingIndicator = true;
    private static boolean bCoachUsingIndicator = false;
    private static boolean bLeftIndicatorOn = false;
    private static boolean bRightIndicatorOn = false;
    private static boolean bIgOn = false; // 是否IG On ，當第一次問到ECU
    // ID時旗標設為TRUE，否則設為FALSE掛住
    private static boolean IG_OFF_ON_RESTART = false; // IG是否重新啟動

    private long start_time, end_time;
    long cal_time_1, cal_time_2;
    int totalDataCount = 0;

    // public save_data_thread save_data_Thread;
    private Send_File_Thread send_file_Thread;

    public Context global_context;

    boolean WriteFileThread_start = false;

    static int readCount = 0;

    private SquareView mSquareView;
    private GaugeView mGauge;
    private RPMGaugeView mRpmgauge;
    private ListView listView;
    private static byte[] wordNeedSplitOuter;
    List<HashMap<String, String>> data = new ArrayList<HashMap<String, String>>();
    private SimpleAdapter adapter;

    // Drawable
    Drawable drLeftIndicatorGreen;
    Drawable drLeftIndicatorRedGreen;
    Drawable drLeftIndicatorRed;
    Drawable drRightIndicatorGreen;
    Drawable drRightIndicatorRedGreen;
    Drawable drRightIndicatorRed;
    Drawable drLeftIndicatorOff;
    Drawable drRightIndicatorOff;
    Drawable drParkOn;
    Drawable drParkOff;
    Drawable drClutchOn;
    Drawable drClutchOff;
    Drawable drSeatOn;
    Drawable drSeatoff;
    Drawable drBrakeOn;
    Drawable drBrakeOff;
    Drawable drReverseOn;
    Drawable drReverseOff;
    Drawable drDoorOn;
    Drawable drDoorOff;
    Drawable drHillStartAssistControlOn;
    Drawable drHillStartAssistControlOff;
    Drawable drHightLampOn;
    Drawable drHightLampOff;
    Drawable drRoomLampOn;
    Drawable drRoomLampOff;
    Drawable drWifiX;
    Drawable drWifi1;
    Drawable drWifi2;
    Drawable drWifi3;
    Drawable drWifi4;

    private boolean LeftOn = false;
    private boolean RightOn = false;
    private static boolean IndicatorOn = false;
    private static boolean bLeftFlashOn = false;
    private static boolean bRightFlashOn = false;
    private static boolean bAllFlashOn = false;

    //RoomLamp 按鈕旗標
    private static boolean isBtnRoomLampNull = true;
    private static boolean isBtnRoomLampIsChecked = false;

    // Buttons
    private Button btnExit;
    private Button btnBrake;
    private Button btnPark;
    private Button btnClutch;
    private Button btnReverse;
    private Button btnSeat;
    private ToggleButton btnLeftIndicator;
    private ToggleButton btnRightIndicator;
    private Button btnHorn;
    private Button btnDoor;
    private Button btnHAC;
    private Button btnHightLamp;
    private ToggleButton btnRoomLamp;
    private Button btnWifi;

    private TextView tvTime;

    // Alert Dialog
    private AlertDialog warningDialog = null;
    private AlertDialog exitDialog = null;
    private AlertDialog engineStallWarningDialog = null;

    // Get current system language
    public static final String CURRENT_LANGUAGE = CustomLocale.getCurrentLauguage();
    public static final String CURRENT_COUNTRY = CustomLocale.getCurrentCountry().toUpperCase();
    public static String APP_VERSION = null;
    
    private LogManager logcatManager = null;

    /// 添加圖標
    private void addShortcut() {
        // 假設第一個 Activity 為 MainActivity(請自行修改)
        Intent shortcutIntent = new Intent(getApplicationContext(), MainActivity.class);
        shortcutIntent.setAction(Intent.ACTION_MAIN);

        // 設定 APP Icon
        // Intent.ShortcutIconResource iconResource =
        // Intent.ShortcutIconResource.fromContext(this,R.drawable.icon);
        Intent.ShortcutIconResource iconResource = Intent.ShortcutIconResource.fromContext(this,
                R.drawable.ic_launcher);
        // Intent.ShortcutIconResource iconResource =
        // Intent.ShortcutIconResource.fromContext(this,R.drawable.hino_icon2);

        if (AppAttributes.CAR_TYPE == CarType.TRUCK) {
            AppName = "VC Truck USB";
        } else {
            AppName = "VC PassengerCar USB";
        }

        Intent intent = new Intent();
        intent.putExtra(Intent.EXTRA_SHORTCUT_INTENT, shortcutIntent);
        intent.putExtra(Intent.EXTRA_SHORTCUT_NAME, AppName);
        intent.putExtra(Intent.EXTRA_SHORTCUT_ICON_RESOURCE, iconResource);
        intent.setAction("com.android.launcher.action.INSTALL_SHORTCUT");
        sendBroadcast(intent);
    }

    /// 儲存設定檔 Config.xml - App 是否安裝
    /// <param name="installed"></param>
    private void setInstalledConfigInfo(boolean installed) {

        if (installed == true) {
            SharedPreferences settings = getSharedPreferences("Config", Activity.MODE_PRIVATE);
            settings.edit()
            .putBoolean("Installed", installed).commit();
        }
    }

    /// 讀取設定檔 Config.xml - App 是否安裝
    private boolean getInstalledConfigInfo() {
        SharedPreferences settings = getSharedPreferences("Config", Activity.MODE_PRIVATE);
        return settings.getBoolean("Installed", false);
    }
    
  /// 讀取設定檔 Config.xml - 設定是否熄火
  	private void setEngineStallConfigInfo(boolean status) {
  		SharedPreferences settings = getSharedPreferences("Config", Activity.MODE_PRIVATE);
  		settings.edit().putBoolean("IsEngineStall", status).commit();
  	}
  	
  	/// 讀取設定檔 Config.xml - 讀取前一次是否熄火
  	private boolean getEngineStallConfigInfo() {
  		SharedPreferences settings = getSharedPreferences("Config", Activity.MODE_PRIVATE);
  		return settings.getBoolean("IsEngineStall", false);
  	}	

    // 設定方向燈燈號 Truck On = 1 , off = 0 ; Toyota on = 0 , off = 1
    private void settingIndicatorFlagAttr() {
        if (AppAttributes.CAR_TYPE == AppAttributes.CarType.TRUCK) {
            INDICATOR_ON = TRUCK_INDICATOR_ON;
            INDICATOR_OFF = TRUCK_INDICATOR_OFF;
        } else {
            INDICATOR_ON = SEDAN_INDICATOR_ON;
            INDICATOR_OFF = SEDAN_INDICATOR_OFF;
        }
    }

    private int getMainViewID() {
        int mainViewID = R.layout.truck_asus_1280_800_main;

        switch (AppAttributes.CAR_TYPE) {
            // Passenger Car
            case PASSENGER_CAR:
                if (AppAttributes.TABLET_TYPE == TabletType.SONYZ2)
                    mainViewID = R.layout.truck_sony_z2_main;
                else if (AppAttributes.TABLET_TYPE == TabletType.SONYZ3)
                    mainViewID = R.layout.truck_sony_z3_main_new;
                else
                    mainViewID = R.layout.truck_asus_1280_800_main;
                break;

            // Truck
            default:
                break;
        }

        return mainViewID;
    }

    private void createDbStoreDirPath() {

        if (AppAttributes.CAR_TYPE == CarType.TRUCK) {
            dirName = truckDirName;
            _DBName = _TruckDBName;
        } else {
            dirName = sedanDirName;
            _DBName = _SedanDBName;
        }

        dbStoreDir = Environment.getExternalStorageDirectory() + File.separator + dirName + File.separator + "DB";
        // + File.separator + _DBName ;

        File file = new File(dbStoreDir);
        if (!file.exists()) {
            file.mkdirs();
        } else {
            file.setExecutable(true);
        }
    }

    public static DBHelper getDBHelper() {
        return dbHelper;
    }

    private void openDB() {
        if (AppAttributes.CAR_TYPE == CarType.TRUCK)
            dbHelper = new TruckDBHelper(this, dbStoreDir + File.separator + _DBName, null, _DBVersion);
        else
            dbHelper = new SedanDBHelper(this, dbStoreDir + File.separator + _DBName, null, _DBVersion);
    }

    private void closeDB() {
        dbHelper.close();
    }

    private boolean isSettingTableHasDefaultValue() {
        boolean settingOk = false;
        if (dbHelper == null)
            return false;

        String sql = "SELECT setting_table_id from SettingTable WHERE setting_table_id = 'S_00000000'";
        Cursor cursor = dbHelper.query(sql);
        if (cursor.getCount() == 0) {
            dbHelper.addValue(settingValue.getDefaultValue());
        }

        return settingOk;
    }

    // //xml 設定 LANDSCAPE，程式會執行Surface.ROTATION_90
    // public void lockScreenOrientation() {
    // switch (((WindowManager) getSystemService(WINDOW_SERVICE))
    // .getDefaultDisplay().getRotation()) {
    // case Surface.ROTATION_90:
    // //setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE);
    // setRequestedOrientation(8/* reverseLandscape */);
    // break;
    // case Surface.ROTATION_180:
    // setRequestedOrientation(9/* reversePortait */);
    // break;
    // case Surface.ROTATION_270:
    // setRequestedOrientation(8/* reverseLandscape */ );
    // break;
    // default :
    // setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_PORTRAIT);
    // }
    // }


    /**
     * Called when the activity is first created.
     */
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        getWindow().addFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);

        CrashHandler crashHandler = CrashHandler.getInstance();
        // 註冊crashHandler
        crashHandler.init(getApplicationContext());

        
        logcatManager = new LogManager();
        
        // try {//製造bug
        // File file = new File(Environment.getExternalStorageState()
        // ,"crash.bin");
        // FileInputStream fis = new FileInputStream(file);
        // byte[] buffer = new byte[1024];
        // fis.read(buffer);
        // } catch (Exception e) {
        // //這裡不能再向上拋異常，如果想要將log信息保存起來，則拋出runtime異常，
        //// 讓自定義的handler來捕獲，統一將文件保存起來上傳
        // throw new RuntimeException(e);
        // }

        // GET ANDROID SERIAL NUMBER
        androidSerialNumber = android.os.Build.SERIAL;
        Log.d(CLASSNAME, "androidSerialNumber : " + androidSerialNumber);

        ProgramInfo info = new ProgramInfo(getApplicationContext());
        APP_VERSION = info.getProgramVersion();
        settingValue = settingValue.getDefaultValue();
        // create db store path
        createDbStoreDirPath();
        openDB();
        isSettingTableHasDefaultValue();

        global_context = this;
        activity = this;
        setContentView(R.layout.main);
        // setContentView(R.layout.truck_asus_1280_800_main);

        Log.d(CLASSNAME, "Current System Language : " + CURRENT_LANGUAGE);
        Log.d(CLASSNAME, "Current System Country : " + CURRENT_COUNTRY);

        
        initFragments();

        if (AppAttributes.CAR_TYPE == AppAttributes.CarType.PASSENGER_CAR) {
            showFragment(mainviewFragment_4th, "MAIN_FRAGMENT_4TH");
            CurrentPage = PageName.MAIN_FRAGMENT_4TH;
        } else {
            if (AppAttributes.APP_MODE == AppMode.Standard) {
                showFragment(mainview_fragment, "MAIN_FRAGMENT");
                CurrentPage = PageName.Main;
            } else {
                showFragment(optionFragment, "OPTION_FRAGMENT");
                CurrentPage = PageName.Option;
            }
        }

        // if(appMode == AppMode.Optional)
        // {
        // //showFragment(logoFragment,"LOGO_FRAGMENT");
        // showFragment(gauges_fragment,"GAUGE_FRAGMENT");
        // }
        // else
//		showFragment(mainview_fragment,"MAIN_FRAGMENT");

        // 設定方向燈燈號屬性
        settingIndicatorFlagAttr();

        // 初始化Alert Dialog
        InitialAlertDialog();
        // 彈出警告重連視窗
        warningDialog.show();

        // MP3 PLAYER
        mp3Player = new MP3Player(this);

        // #region 添加圖標
        // AddShortcut();
        boolean bAppInstalled = getInstalledConfigInfo();
        if (!bAppInstalled) {
            addShortcut();
            setInstalledConfigInfo(true);
        }
        bAppInstalled = getInstalledConfigInfo();
        // #endregion
        
        // #region 確定是否熄火
        boolean bIsEngineStall = getEngineStallConfigInfo();
 		if(bIsEngineStall)
 		{
 			//engineStallWarningDialog.show();
 		}
 		// #endregion	

        // Time View
        tvTime = (TextView) findViewById(R.id.currtime_text_view1);

        // listView = (ListView)findViewById(R.id.listview);
        // if(listView != null)
        // listView.setAdapter(adapter);
        //
        // adapter=new SimpleAdapter(this, data, R.layout.item
        // , new String[]{"item","value","unit"},new
        // int[]{R.id.all_data,R.id.value,R.id.unit});

        // sendfileButton1 = (Button)findViewById(R.id.SendFileButton1);
        // pauseButton = (Button)findViewById(R.id.PauseButton);
        // sendactcmdButton = (Button)findViewById(R.id.SendActButton);
        // exitButton = (Button)findViewById(R.id.ExitButton);
        /* allocate buffer */
        // writeBuffer = new byte[64];
        readBuffer = new byte[4096];
        actualNumBytes = new int[1];
        uartInterface = new FT311UARTInterface(this, null, warningDialog);

        // mGauge = (GaugeView) findViewById(R.id.gauge_view1);
        // mRpmgauge = (RPMGaugeView) findViewById(R.id.rpmgauge_view1);
        // mSquareView = (SquareView) findViewById(R.id.square_view1);
        //

        boolean hasGuiThread = false;
        for (Thread t : threadList) {
            if (t.getName().equals("Gui")) {
                if (t.isAlive())
                    hasGuiThread = true;
                else {
                    threadList.remove(t);
                    hasGuiThread = false;
                }
                break;
            }
        }

        if (!hasGuiThread) {
            // Start Timer Thread
            Thread timerThread = new Thread(new Runnable() {

                @Override
                public void run() {
                    // TODO Auto-generated method stub
                    UpdateTimerValue();
                }
            });
            timerThread.setName("Gui");
            threadList.add(timerThread);
            timerThread.start();
        }

        // //Button Initial
        // InitialButton();
        //
        // //Button Event Setting
        // ButtonEventSetting();
        //
        // //Drawable Initial
        // InitialDrawable();

        try {
            fos_open = new FileInputStream(ACCESS_FILE);
        } catch (FileNotFoundException e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        }
        if (send_file_Thread == null) {
            send_file_Thread = new Send_File_Thread(handler, fos_open);
            send_file_Thread.start();
        }

    }

    private void initFragments() {

        if (AppAttributes.CAR_TYPE == CarType.TRUCK) {
            optionFragment = new OptionFragment();
            logoFragment = new LogoFragment();
            settingTableFragment = new SettingTableFragment();
            // mainview_fragment = new MainviewFragment();
            mainview_fragment = new MainviewFragment();
            //p2_fragment = new Page2Fragment();
            gauges_fragment = new GaugesFragment();
            results_fragment = new ResultsFragment();

            fragmentList.add(optionFragment);
            fragmentList.add(logoFragment);
            fragmentList.add(settingTableFragment);
            fragmentList.add(mainview_fragment);
            fragmentList.add(gauges_fragment);
            fragmentList.add(results_fragment);
            fragmentList.add(p2_fragment);
        } else {
            mainviewFragment_4th = new MainviewFragment_4th();
            fragmentList.add(mainviewFragment_4th);
        }

    }

    private void showFragment(Fragment fragment, String framentTag) {
        FragmentManager fragmentManager = getFragmentManager();
        FragmentTransaction fragmentTransaction = fragmentManager.beginTransaction();
        // Fragment fragment = fragmentManager.findFragmentByTag(framentTag);

        if (fragment == null)
            return;

        if (!fragment.isAdded()) {
            fragmentTransaction.add(R.id.framelayout, fragment, framentTag);
//            fragmentTransaction.addToBackStack(null);
        }
        if (fragment.isHidden())
            fragmentTransaction.show(fragment);

        // Hide all other fragment
        String listCurrentFragmentTag = "";
        for (int i = 0; i < fragmentList.size(); i++) {
            fragment = fragmentList.get(i);
            {
                if (fragment != null) {
                    listCurrentFragmentTag = fragment.getTag();
                    if (listCurrentFragmentTag == null) {
                        fragmentTransaction.hide(fragment);
                        // fragmentTransaction.commit();
                    } else {
                        if (!listCurrentFragmentTag.equals(framentTag)) {
                            fragmentTransaction.hide(fragment);
                            // fragmentTransaction.commit();
                        }
                    }
                }
                try {
                    fragmentTransaction.commit();
                } catch (Exception ex) {
                    ex.printStackTrace();
                }
            }
        }
        // fragmentTransaction.commit();
    }

    private Menu menu;

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.main, menu);
        this.menu = menu;
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();
        if (id == R.id.action_settings) {
            Toast.makeText(activity, "Setting Pressed.", Toast.LENGTH_SHORT).show();
            return true;
        } else if (id == R.id.action_changePage2) {
            MenuItem menuItem = menu.findItem(R.id.action_changePage2);
            String title = "" + menuItem.getTitle();
            FragmentManager fragmentManager = getFragmentManager();
            FragmentTransaction fragmentTransaction = fragmentManager.beginTransaction();
            if (title.contains("2")) {
                //// if(!p2_fragment.isAdded())
                //// {
                //// fragmentTransaction.replace(R.id.mainview_fragment,
                //// p2_fragment,"P2_FRAGMENT");
                //// fragmentTransaction.hide(mainview_fragment);
                //// fragmentTransaction.addToBackStack(null);
                //// }
                //// else
                //// {
                //// if(p2_fragment.isHidden())
                //// {
                //// fragmentTransaction.show(p2_fragment);
                //// }
                //// fragmentTransaction.hide(mainview_fragment);
                //// }
                // //fragmentTransaction.hide(mainview_fragment);
                //
                //
                // fragmentTransaction.replace(R.id.framelayout, p2_fragment);
                // fragmentTransaction.addToBackStack(null);

                if (!p2_fragment.isAdded()) {
                    // fragmentTransaction.add(R.id.framelayout, p2_fragment ,
                    // "P2_FRAGMENT");
                    // fragmentTransaction.hide(mainview_fragment);
                    // if(p2_fragment.isHidden())
                    // fragmentTransaction.show(p2_fragment);
                    showFragment(p2_fragment, "P2_FRAGMENT");
                } else {
                    // fragmentTransaction.hide(mainview_fragment);
                    // if(p2_fragment.isHidden())
                    // fragmentTransaction.show(p2_fragment);
                    showFragment(p2_fragment, "P2_FRAGMENT");

                }

            } else {
                // fragmentTransaction.replace(R.id.framelayout,
                // mainview_fragment);
                // fragmentTransaction.addToBackStack(null);

                // fragmentTransaction.hide(p2_fragment);
                // if(mainview_fragment.isHidden())
                // fragmentTransaction.show(mainview_fragment);
                showFragment(mainview_fragment, "MAIN_FRAGMENT");
            }
            fragmentTransaction.commit();
            updateMenuTitles();
            return true;
        }

        return super.onOptionsItemSelected(item);
    }

    private void updateMenuTitles() {
        MenuItem menuItem = menu.findItem(R.id.action_changePage2);
        String title = "" + menuItem.getTitle();
        if (title.contains("1")) {
            title = title.replace("1", "2");
            menuItem.setTitle(title);
        } else {
            title = title.replace("2", "1");
            menuItem.setTitle(title);
        }

        // if (inBed) {
        // bedMenuItem.setTitle(outOfBedMenuTitle);
        // } else {
        // bedMenuItem.setTitle(inBedMenuTitle);
        // }
    }

    private void InitialExitDialog() {
        if (exitDialog != null)
            return;

        exitDialog = new AlertDialog.Builder(MainActivity.this).create();
        exitDialog.setCanceledOnTouchOutside(false);
        exitDialog.setIcon(R.drawable.ic_launcher);
        // exitDialog.setTitle(exitTitle);
        // exitDialog.setMessage(exitMessage);
        exitDialog.setTitle(getResources().getText(R.string.exitTitle));
        exitDialog.setMessage(getResources().getText(R.string.exitMessage));
        exitDialog.setButton(DialogInterface.BUTTON_POSITIVE, getResources().getText(R.string.YES),
                new DialogInterface.OnClickListener() {

                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        // TODO Auto-generated method stub
                        // onDestroy();
                        CloseApp();
                    }
                });
        exitDialog.setButton(DialogInterface.BUTTON_NEGATIVE, getResources().getText(R.string.NO),
                new DialogInterface.OnClickListener() {

                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        // TODO Auto-generated method stub
                        if (exitDialog != null) {
                            if (exitDialog.isShowing())
                                exitDialog.dismiss();
                        }
                    }
                });

        // Dialog 加入 back按鍵無效化
        exitDialog.setOnKeyListener(new DialogInterface.OnKeyListener() {

            @Override
            public boolean onKey(DialogInterface dialog, int keyCode, KeyEvent event) {
                // TODO Auto-generated method stub
                if (keyCode == KeyEvent.KEYCODE_BACK) {
                    // dialog.cancel();
                    return true;
                }
                return false;
            }
        });
    }

    private void InitialWarningDialog() {
        if (warningDialog != null)
            return;

        warningDialog = new AlertDialog.Builder(MainActivity.this).create();
        warningDialog.setCanceledOnTouchOutside(false);
        // warningDialog.setTitle(warningTitle);
        // warningDialog.setMessage(warningMessage);
        warningDialog.setTitle(getResources().getText(R.string.warningTitle));
        warningDialog.setMessage(getResources().getText(R.string.warningMessage));
        warningDialog.setButton(DialogInterface.BUTTON_POSITIVE, getResources().getText(R.string.OK),
                new DialogInterface.OnClickListener() {

                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        // TODO Auto-generated method stub
                        onDestroy();
                    }
                });

        // Dialog 加入 back按鍵無效化
        warningDialog.setOnKeyListener(new DialogInterface.OnKeyListener() {

            @Override
            public boolean onKey(DialogInterface dialog, int keyCode, KeyEvent event) {
                // TODO Auto-generated method stub
                if (keyCode == KeyEvent.KEYCODE_BACK) {
                    // dialog.cancel();
                    return true;
                }
                return false;
            }
        });
    }
    
//    private void InitialEngineStallWarningDialog()
//	{
//		if(engineStallWarningDialog != null)
//			return;
//		
//		engineStallWarningDialog = new AlertDialog.Builder(MainActivity.this).create();
//		engineStallWarningDialog.setCanceledOnTouchOutside(false);
//		// warningDialog.setTitle(warningTitle);
//		// warningDialog.setMessage(warningMessage);
//		engineStallWarningDialog.setTitle(getResources().getText(R.string.warningTitle));
//		engineStallWarningDialog.setMessage(getResources().getText(R.string.engineStallWarningMessage));
//		engineStallWarningDialog.setButton(DialogInterface.BUTTON_POSITIVE, getResources().getText(R.string.OK),
//				new DialogInterface.OnClickListener() {
//
//					@Override
//					public void onClick(DialogInterface dialog, int which) {
//						// TODO Auto-generated method stub
//						setEngineStallConfigInfo(false);
//						synchronized (MonitorStates.LOCKER) {
//							MonitorStates.isResume = true;
//						}
//						
//						runOnUiThread(new Runnable() {
//							public void run() {
//								if(AppAttributes.CAR_TYPE == CarType.PASSENGER_CAR)
//								{
//									resultsFragment_4th.setUIResume();
//								}
//							}
//						});
//					}
//				});
//		
//		engineStallWarningDialog.setButton(DialogInterface.BUTTON_NEGATIVE, "NO",
//				new DialogInterface.OnClickListener() {
//
//					@Override
//					public void onClick(DialogInterface dialog, int which) {
//						// TODO Auto-generated method stub
//						setEngineStallConfigInfo(false);
//					}
//				});
//
//		// Dialog 加入 back按鍵無效化
//		engineStallWarningDialog.setOnKeyListener(new DialogInterface.OnKeyListener() {
//
//			@Override
//			public boolean onKey(DialogInterface dialog, int keyCode, KeyEvent event) {
//				// TODO Auto-generated method stub
//				if (keyCode == KeyEvent.KEYCODE_BACK) {
//					// dialog.cancel();
//					return true;
//				}
//				return false;
//			}
//		});
//	}
//

    private void InitialAlertDialog() {
        InitialWarningDialog();
        InitialExitDialog();
        //InitialEngineStallWarningDialog();
    }

    // final SimpleDateFormat sdfDate = new SimpleDateFormat("yyyy-MM-dd
    // HH:mm:ss.SSS");
    final static SimpleDateFormat sdfTime = new SimpleDateFormat("HH:mm");
    final static SimpleDateFormat sdfDate = new SimpleDateFormat("yyyy/MM/dd");
    final static SimpleDateFormat sdfDateFull = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss.sss");//dd/MM/yyyy

    public static String getCurrentTimeFull() {
        Date now = new Date();
        String strDate = sdfDateFull.format(now);
        return strDate;
    }
    
    public static String getCurrentTime() {
        Date now = new Date();
        String strDate = sdfTime.format(now);
        return strDate;
    }

    public static String getCurrentDate() {
        Date now = new Date();
        String strDate = sdfDate.format(now);
        return strDate;
    }

    private void InitialButtonTest() {
        if (sendfileButton1 != null)
            sendfileButton1.setOnClickListener(new View.OnClickListener() {
                // @Override
                public void onClick(View v) {
                    synchronized (LOCKER) {
                        data.clear();
                        try {
                            Thread.sleep(100);
                        } catch (InterruptedException e) {
                            // TODO Auto-generated catch block
                            e.printStackTrace();
                        }
                    }
                    if (uartInterface.hasUsbPermission()) {

                        // adapter.notifyDataSetChanged();
                        isAppPause = false;
                        sendfileButton1.setEnabled(false);

                        try {
                            fos_open = new FileInputStream(ACCESS_FILE);
                        } catch (FileNotFoundException e) {
                            // TODO Auto-generated catch block
                            e.printStackTrace();
                        }
                        if (send_file_Thread == null) {
                            send_file_Thread = new Send_File_Thread(handler, fos_open);
                            send_file_Thread.start();
                        }
                    }

                }
            });

        if (pauseButton != null)
            pauseButton.setOnClickListener(new View.OnClickListener() {

                @Override
                public void onClick(View v) {
                    // TODO Auto-generated method stub
                    isAppPause = true;
                    sendfileButton1.setEnabled(true);
                }
            });

        if (sendactcmdButton != null)
            sendactcmdButton.setOnClickListener(new View.OnClickListener() {

                @Override
                public void onClick(View v) {
                    // TODO Auto-generated method stub
                    synchronized (ACT_HORN_ACTIVE_LOCKER) {
                        IsHornAct = true;
                    }
                }
            });

        if (exitButton != null)
            exitButton.setOnClickListener(new View.OnClickListener() {

                @Override
                public void onClick(View v) {
                    // TODO Auto-generated method stub
                    isAppPause = true;
                    try {
                        Thread.sleep(100);
                    } catch (InterruptedException e) {
                        // TODO Auto-generated catch block
                        e.printStackTrace();
                    }
                    uartInterface.DestroyAccessory(true);
                    android.os.Process.killProcess(android.os.Process.myPid());
                }
            });
    }

    private void InitialButton() {
        // Button Initial
        // btnExit = (Button)findViewById(R.id.btnExit);
        btnBrake = (Button) findViewById(R.id.btnBrake);
        btnPark = (Button) findViewById(R.id.btnPark);
        btnClutch = (Button) findViewById(R.id.btnClutch);
        btnLeftIndicator = (ToggleButton) findViewById(R.id.btnLeftIndicator);
        btnLeftIndicator.setTag("OFF");
        btnLeftIndicator.setTextOff("");
        btnLeftIndicator.setTextOn("");
        btnReverse = (Button) findViewById(R.id.btnReverse);
        btnSeat = (Button) findViewById(R.id.btnSeat);
        btnRightIndicator = (ToggleButton) findViewById(R.id.btnRightIndicator);
        btnRightIndicator.setTag("OFF");
        btnRightIndicator.setTextOff("");
        btnRightIndicator.setTextOn("");
        btnHorn = (Button) findViewById(R.id.btnHorn);
        btnDoor = (Button) findViewById(R.id.btnDoor);

        // VER1.0.1取消方向燈可按
        if (AppAttributes.CAR_TYPE == CarType.TRUCK) {
            btnRightIndicator.setEnabled(false);
            btnLeftIndicator.setEnabled(false);
        }

        // btnHAC = (ToggleButton)findViewById(R.id.btnRightIndicator);
        // btnHightLamp = (ToggleButton)findViewById(R.id.btnRightIndicator);
        // btnRoomLamp = (ToggleButton)findViewById(R.id.btnRightIndicator);

        btnWifi = (Button) findViewById(R.id.btnWifi);

        // btnExit.Enabled = false;
        btnBrake.setEnabled(false);
        btnPark.setEnabled(false);
        btnClutch.setEnabled(false);
        if (btnReverse != null)
            btnReverse.setEnabled(false);

        if (btnSeat != null)
            btnSeat.setEnabled(false);
    }

    private void ButtonEventSetting() {
        // 喇叭按鈕 點擊事件響應
        btnHorn.setOnClickListener(new View.OnClickListener() {

            @Override
            public void onClick(View v) {
                // TODO Auto-generated method stub
                synchronized (ACT_HORN_ACTIVE_LOCKER) {
                    IsHornAct = true;
                }
            }
        });

        if (btnRoomLamp != null) {
            btnRoomLamp.setOnClickListener(new View.OnClickListener() {

                @Override
                public void onClick(View v) {
                    // TODO Auto-generated method stub
                    IsRoomLampCloseOK = false;
                }
            });

            btnRoomLamp.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {

                @TargetApi(Build.VERSION_CODES.JELLY_BEAN)
                @SuppressLint("NewApi")
                @Override
                public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
                    // TODO Auto-generated method stub
                    if (btnRoomLamp.isChecked()) {
                        btnRoomLamp.setBackground(drRoomLampOn);
                    } else {
                        btnRoomLamp.setBackground(drRoomLampOff);
                    }
                }
            });
        }

//		btnRightIndicator.setOnClickListener(new View.OnClickListener() {
//
//			@SuppressLint("NewApi")
//			@TargetApi(Build.VERSION_CODES.JELLY_BEAN)
//			@Override
//			public void onClick(View v) {
//				// TODO Auto-generated method stub
//				btnRightIndicator.setTag("ON");
//				IsRightIndicatorCloseOK = false;
//				IsRightIndicatorButtonOn = true;
//				if (bCanUsingIndicator == true) {
//					if (btnRightIndicator.isChecked()) {
//						btnRightIndicator.setBackground(drRightIndicatorRed);
//						// btnLeftIndicator.Checked = false;
//						btnLeftIndicator.setChecked(!btnRightIndicator.isChecked());
//					} else {
//						btnRightIndicator.setBackground(drRightIndicatorOff);
//					}
//				}
//			}
//		});
//
//		btnRightIndicator.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
//
//			@TargetApi(Build.VERSION_CODES.JELLY_BEAN)
//			@SuppressLint("NewApi")
//			@Override
//			public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
//				// TODO Auto-generated method stub
//				if (btnRightIndicator.isChecked()) {
//					btnRightIndicator.setBackground(drRightIndicatorRed);
//					btnLeftIndicator.setChecked(false);
//				} else {
//					btnRightIndicator.setBackground(drRightIndicatorOff);
//				}
//			}
//		});
//
//		btnLeftIndicator.setOnClickListener(new View.OnClickListener() {
//
//			@TargetApi(Build.VERSION_CODES.JELLY_BEAN)
//			@SuppressLint("NewApi")
//			@Override
//			public void onClick(View v) {
//				// TODO Auto-generated method stub
//				btnLeftIndicator.setTag("ON");
//				IsLeftIndicatorCloseOK = false;
//				IsLeftIndicatorButtonOn = true;
//				if (bCanUsingIndicator == true) {
//					if (btnLeftIndicator.isChecked()) {
//						btnLeftIndicator.setBackground(drLeftIndicatorRed);
//						// btnRightIndicator.Checked = false;
//						btnRightIndicator.setChecked(!btnLeftIndicator.isChecked());
//					} else {
//						btnLeftIndicator.setBackground(drLeftIndicatorOff);
//					}
//				}
//			}
//		});
//
//		btnLeftIndicator.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
//
//			@TargetApi(Build.VERSION_CODES.JELLY_BEAN)
//			@SuppressLint("NewApi")
//			@Override
//			public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
//				// TODO Auto-generated method stub
//				if (btnLeftIndicator.isChecked()) {
//					btnLeftIndicator.setBackground(drLeftIndicatorRed);
//					btnRightIndicator.setChecked(false);
//				} else {
//					btnLeftIndicator.setBackground(drLeftIndicatorOff);
//				}
//			}
//		});

        // 離開按鈕 點擊事件響應
        if (btnExit != null)
            btnExit.setOnClickListener(new View.OnClickListener() {

                @Override
                public void onClick(View v) {

                    // TODO Auto-generated method stub
                    if (!exitDialog.isShowing())
                        exitDialog.show();

                    // isAppExit = true;
                    // try {
                    // Thread.sleep(100);
                    // } catch (InterruptedException e) {
                    // // TODO Auto-generated catch block
                    // e.printStackTrace();
                    // }
                    // uartInterface.DestroyAccessory(true);
                    // try {
                    // Thread.sleep(100);
                    // } catch (InterruptedException e) {
                    // // TODO Auto-generated catch block
                    // e.printStackTrace();
                    // }
                    // android.os.Process.killProcess(android.os.Process.myPid());
                }
            });
    }

    private void InitialDrawable() {
        // Drawable Initial
        drLeftIndicatorGreen = getResources().getDrawable(R.drawable.leftongreen);
        drLeftIndicatorRedGreen = getResources().getDrawable(R.drawable.leftonredgreen);
        drLeftIndicatorRed = getResources().getDrawable(R.drawable.leftonred);
        drRightIndicatorGreen = getResources().getDrawable(R.drawable.rightongreen);
        drRightIndicatorRedGreen = getResources().getDrawable(R.drawable.rightonredgreen);
        drRightIndicatorRed = getResources().getDrawable(R.drawable.rightonred);
        drLeftIndicatorOff = getResources().getDrawable(R.drawable.leftoff);
        drRightIndicatorOff = getResources().getDrawable(R.drawable.rightoff);
        drParkOn = getResources().getDrawable(R.drawable.pon);
        drParkOff = getResources().getDrawable(R.drawable.poff);
        drBrakeOn = getResources().getDrawable(R.drawable.brakeon);
        drBrakeOff = getResources().getDrawable(R.drawable.brakeoff);
        drClutchOn = getResources().getDrawable(R.drawable.clutchon);
        drClutchOff = getResources().getDrawable(R.drawable.clutchoff);
        // drSeatOn = getResources().getDrawable(R.drawable.seatBeltGreenOn);
        // drSeatoff = getResources().getDrawable(R.drawable.seatBeltGreenOff);
        drSeatOn = getResources().getDrawable(R.drawable.seatbeltredon);
        drSeatoff = getResources().getDrawable(R.drawable.seatbeltredoff);
        drReverseOn = getResources().getDrawable(R.drawable.ron);
        drReverseOff = getResources().getDrawable(R.drawable.roff);
        drDoorOn = getResources().getDrawable(R.drawable.dooropen);
        drDoorOff = getResources().getDrawable(R.drawable.doorclose);

        drWifiX = getResources().getDrawable(R.drawable.wifix);
        drWifi1 = getResources().getDrawable(R.drawable.wifi1);
        drWifi2 = getResources().getDrawable(R.drawable.wifi2);
        drWifi3 = getResources().getDrawable(R.drawable.wifi3);
        drWifi4 = getResources().getDrawable(R.drawable.wifi4);
    }
    //
    // final protected static char[] hexArray =
    // "0123456789ABCDEF".toCharArray();
    // public String bytesToHex(byte[] bytes) {
    // char[] hexChars = new char[bytes.length * 2];
    // for ( int j = 0; j < bytes.length; j++ ) {
    // int v = bytes[j] & 0xFF;
    // hexChars[j * 2] = hexArray[v >>> 4];
    // hexChars[j * 2 + 1] = hexArray[v & 0x0F];
    // }
    // return new String(hexChars);
    // }
    //
    // public String bytesToHex(byte[] bytes,int length) {
    // char[] hexChars = new char[length * 2];
    // for ( int j = 0; j < length; j++ ) {
    // int v = bytes[j] & 0xFF;
    // hexChars[j * 2] = hexArray[v >>> 4];
    // hexChars[j * 2 + 1] = hexArray[v & 0x0F];
    // }
    // return new String(hexChars);
    // }

    @Override
    protected void onResume() {
        // Ideally should implement onResume() and onPause()
        // to take appropriate action when the activity looses focus
        super.onResume();
        isAppPause = false;
        uartInterface.ResumeAccessory();
    }

    @Override
    protected void onPause() {
        // Ideally should implement onResume() and onPause()
        // to take appropriate action when the activity looses focus
        super.onPause();
        isAppPause = true;
        
        if (uartInterface != null) {
            if (uartInterface.hasUsbPermission())
                onDestroy();

        }
        // onDestroy();

    }
    
    public void updateAndExportDb2Csv()
    {
    	if(dbHelper == null)
    		return;
    	//Update DB File for PC
    	File dbFullpathFile = dbHelper.getDbFullpathFile();
    	if(dbFullpathFile != null)
    		new SingleMediaScanner(activity.getApplicationContext(), dbFullpathFile);
    	dbHelper.exportCsv();
    }
    
    public void updateAndExportDb2Excel()
    {
    	if(dbHelper == null)
    		return;
    	//Update DB File for PC
    	File dbFullpathFile = dbHelper.getDbFullpathFile();
    	if(dbFullpathFile != null)
    		new SingleMediaScanner(activity.getApplicationContext(), dbFullpathFile);
    	dbHelper.exportExcel();
    }

    @Override
    protected void onDestroy() {

        if (dbHelper != null) {
            dbHelper.closeCursor();
            closeDB();
        }
        uartInterface.DestroyAccessory(true);
        android.os.Process.killProcess(android.os.Process.myPid());
        super.onDestroy();
    }

    @Override
    protected void onSaveInstanceState(Bundle outState) {
        // TODO Auto-generated method stub
        super.onSaveInstanceState(outState);
    }

    /*********************
     * UI Update
     *************************************************/

    @TargetApi(Build.VERSION_CODES.JELLY_BEAN)
    @SuppressLint("NewApi")
    protected void UpdateTimerValue() {
        while (true) {

            if (CurrentPage == PageName.MAIN_FRAGMENT_4TH) {
                if (btnLeftIndicator == null)
                    btnLeftIndicator = mainviewFragment_4th.getLeftIndicatorButton();
                if (btnRightIndicator == null)
                    btnRightIndicator = mainviewFragment_4th.getRightIndicatorButton();
            }


            if (AppAttributes.CAR_TYPE == CarType.TRUCK) {
                if (mainview_fragment != null) {
                    // 第一頁的資料更新
                    if (MainActivity.CurrentPage == PageName.Main)
                        mainview_fragment.UpdateTimerValue(vdiDatas);
                    else if (MainActivity.CurrentPage == PageName.Monitor_Main)
                        gauges_fragment.UpdateTimerValue(vdiDatas, vdiInfoDatas);
                    else if (MainActivity.CurrentPage == PageName.Result)
                        results_fragment.UpdateTimerValue(vdiInfoDatas, vdiFreezeDatas);
                }

                // //第二頁的資料更新
                // else
                // {
                // if(debug_fragment == null)
                // debug_fragment =
                // (DebugFragment)FragmentManager.FindFragmentByTag("BACK");
                // if (debug_fragment != null)
                // //debug_fragment.UpdateList(VdiDatas);
                // debug_fragment.UpdateList(VdiExternalDatas);
                // }

            } else {
                if (mainviewFragment_4th != null) {
                    mainviewFragment_4th.UpdateTimerValue(vdiDatas, vdiInfoDatas);
                }
            }

            try {
                Thread.sleep(10);
            } catch (InterruptedException e) {
                // TODO Auto-generated catch block
                e.printStackTrace();
            }
        }
    }

    public void ReplaceFragment(Fragment newFragment) {
        FragmentManager fragmentManager = getFragmentManager();
        FragmentTransaction trasection = fragmentManager.beginTransaction();

        if (!newFragment.isAdded()) {
            try {
                trasection.hide(mainview_fragment);
                // FragmentTransaction trasection =
                // FragmentManager.BeginTransaction();
                trasection.add(R.id.framelayout, newFragment, "BACK");
                // trasection.add(Resource.Id.mainview_fragment, newFragment);
                trasection.addToBackStack(null);
                trasection.commit();

                // 添加完順便設定debug_fragment 實例
                // debug_fragment = (Page2Fragment).findFragmentByTag("BACK");
            } catch (Exception e) {
                // TODO: handle exception
                // AppConstants.printLog(e.getMessage());
                // throw e;
            }
        } else {
            // trasection.Hide(mainview_fragment);
            trasection.hide(mainview_fragment);
            trasection.hide(newFragment);
            trasection.show(newFragment);
            trasection.commit();
        }

    }

    public void ShowMainFragment() {
        FragmentManager fragmentManager = getFragmentManager();
        FragmentTransaction trasection = fragmentManager.beginTransaction();

        Fragment backFragment = fragmentManager.findFragmentByTag("BACK");
        trasection.hide(backFragment);
        trasection.show(mainview_fragment);
        trasection.commit();
    }

    /*********************
     * helper routines
     *************************************************/

    final Handler handler = new Handler() {
        @Override
        public void handleMessage(Message msg) {
            switch (msg.what) {
                case UPDATE_READ_DATA: {
                    Double diffime = (double) (cal_time_2 - start_time) / 1000;
                    // TimeText.setText("Get "+totalDataCount + " bytes in " +
                    // diffime.toString() + " sec");
                }
                break;
                case UPDATE_READ_DATA_DONE: {
                    Double diffime = (double) (end_time - start_time) / 1000;
                    // TimeText.setText("Get "+totalDataCount + " bytes in " +
                    // diffime.toString() + " sec");
                }
                break;
                case UPDATE_SEND_DATA: {
                    Double diffime = (double) (cal_time_2 - start_time) / 1000;
                    // TimeText.setText("Send "+totalDataCount + " bytes in " +
                    // diffime.toString() + " sec");
                }
                break;

                case UPDATE_SEND_DATA_DONE: {
                    Toast.makeText(global_context, "Send File - Done", Toast.LENGTH_SHORT).show();
                    Double diffime = (double) (end_time - start_time) / 1000;
                    // TimeText.setText("Send "+totalDataCount + " bytes in " +
                    // diffime.toString() + " sec");
                }
                break;
                default:
                    break;
            }
        }
    };

    public int computeChecksum(int recvLength) {
        int checksum = 0;
        try {
            if (recvLength < 255) {
                for (int i = 0; i < recvLength; i++) {
                    checksum += tmpBuffer[i];
                }
            }
        } catch (Exception ex) {
            return 0;
        }
        return checksum;
    }

    public boolean validChecksum(int recvLength) {
        boolean checksumOK = false;
        byte checksum = (byte) computeChecksum(recvLength - 1);
        try {
            if (checksum == (byte) tmpBuffer[recvLength - 1])
                checksumOK = true;
        } catch (Exception ex) {
            return false;
        }
        return checksumOK;
    }

    /* UNPACK DATAS */
    @SuppressLint("NewApi")
    public synchronized boolean unpackDatas(int recvLength) {
        boolean unpackOK = false;
        if (recvLength < 4)
            return false;

        if (tmpBuffer[2] == (byte) 0x42) {
            unpackOK = unpackFreezeDatas(recvLength);
            return unpackOK;
        } else if (tmpBuffer[2] == (byte) 0x49) {
            unpackOK = unpackInfoDatas(recvLength);
            return unpackOK;
        } else if (tmpBuffer[2] == (byte) 0x6A) {
            unpackOK = unpackVdiDatas(recvLength);
            return unpackOK;
        } else if (tmpBuffer[2] == (byte) 0xFF) {
            isAlreadyInit = false;
            return true;
        }

        return false;
    }

    /* FREEZE DATAS */
    private static HashMap<Integer, Integer> vdiFreezeDatas = new HashMap<Integer, Integer>();
    private ArrayList<Integer> vdiFreezeItems = new ArrayList<Integer>();

    public void initFreezeItemsForTruck() {
        vdiFreezeItems.clear();
        vdiFreezeItems.add(3100);
        vdiFreezeItems.add(3101);
        vdiFreezeItems.add(3102);
        vdiFreezeItems.add(3103);
        for (Integer key : vdiFreezeItems) {
            vdiFreezeDatas.put(key, 0);
        }
    }

    public void initFreezeItemsForSedan() {
        vdiFreezeItems.clear();
        vdiFreezeItems.add(3101);
        vdiFreezeItems.add(3102);
        vdiFreezeItems.add(3120);
        vdiFreezeItems.add(3121);
        vdiFreezeItems.add(3122);
        for (Integer key : vdiFreezeItems) {
            vdiFreezeDatas.put(key, 0);
        }
    }

//	public void initFreezeItems() {
//		vdiFreezeItems.clear();
//		vdiFreezeItems.add(3100);
//		vdiFreezeItems.add(3101);
//		vdiFreezeItems.add(3102);
//		vdiFreezeItems.add(3103);
//		for (Integer key : vdiFreezeItems) {
//			vdiFreezeDatas.put(key, 0);
//		}
//	}

    @SuppressLint("NewApi")
    public synchronized boolean unpackFreezeDatas(int recvLength) {
        boolean unpackOK = false;

        if (recvLength < 4)
            return false;

        // if(tmpBuffer[2] != (byte)0x42)
        // return false;

        if (!((tmpBuffer[2] == (byte) 0x42) || (tmpBuffer[2] == (byte) 0xFF)))
            return false;

        byte[] bytes = null;
        float f = 0.0f;
        int vdiDataCount = 0;
        int lvId = 0;

        try {
            if (validChecksum(recvLength)) {
                // try{
                for (int i = 3, j = 0; i < recvLength - 1; i += 4, j++) {
                    bytes = Arrays.copyOfRange(tmpBuffer, i, i + 4);
                    f = ByteBuffer.wrap(bytes).order(ByteOrder.BIG_ENDIAN).getFloat();

                    // //For Text
                    // HashMap dataMap = data.get(vdiDataCount);
                    // setLvItemDisplayText(dataMap,f);
                    // vdiTextDatas.set(vdiDataCount, dataMap);

                    // For graphical
                    // lvId = vdiDatas.get(vdiLvItems.get(j));
                    lvId = vdiFreezeItems.get(j);
                    vdiFreezeDatas.put(lvId, (int) f);
                    // dataMap.put("value", "" + f);

                    // vdiDataCount++;
                }
                unpackOK = true;
                // }catch(Exception ex)
                // {
                // return false;
                // }
            }
        } catch (Exception e) {
            return false;
        }
        return unpackOK;
    }

    /* INFO DATAS */
    private static HashMap<Integer, Float> vdiInfoDatas = new HashMap<Integer, Float>();
    private ArrayList<Integer> vdiInfoItems = new ArrayList<Integer>();

    public void initInfoItems() {
        vdiInfoItems.clear();
        vdiInfoItems.add(3900);
        vdiInfoItems.add(3901);
        vdiInfoItems.add(3902);
        vdiInfoItems.add(3201);
        for (Integer key : vdiInfoItems) {
            vdiInfoDatas.put(key, 0f);
        }
    }

    // public synchronized int getInfoItem(int recvLength)
    // {
    // vdiInfoItems.clear();
    // int infoItem = 0;
    // int id = 0;
    // if(validChecksum(recvLength))
    // {
    // for (int i = 3; i < recvLength-1; i+=2) {
    // HashMap<String,String> map = new HashMap<String, String>();
    // id = (tmpBuffer[i] << 8) + tmpBuffer[i+1];
    // vdiInfoItems.add(id);
    // map.put("item", ""+id);
    // data.add(map);
    //
    //// ////for text
    //// HashMap<Integer,String> vdiValMap = new HashMap<Integer, String>();
    //// vdiValMap.put(id, "");
    //// vdiTextDatas.add(vdiValMap);
    //
    // //for graphical
    // vdiInfoDatas.put(id,0);
    // infoItem ++;
    // }
    // //adapter.notifyDataSetChanged();
    // }
    //
    // return infoItem;
    // }
    //
    @SuppressLint("NewApi")
    public synchronized boolean unpackInfoDatas(int recvLength) {
        boolean unpackOK = false;
        if (recvLength < 4)
            return false;

        // if(tmpBuffer[2] != (byte)0x49)
        // return false;

        if (!((tmpBuffer[2] == (byte) 0x49) || (tmpBuffer[2] == (byte) 0xFF)))
            return false;

        byte[] bytes = null;
        float f = 0.0f;
        int vdiDataCount = 0;
        int lvId = 0;

        try {
            if (validChecksum(recvLength)) {
                // try{
                for (int i = 3, j = 0; i < recvLength - 1; i += 4, j++) {
                    bytes = Arrays.copyOfRange(tmpBuffer, i, i + 4);
                    f = ByteBuffer.wrap(bytes).order(ByteOrder.BIG_ENDIAN).getFloat();

                    // //For Text
                    // HashMap dataMap = data.get(vdiDataCount);
                    // setLvItemDisplayText(dataMap,f);
                    // vdiTextDatas.set(vdiDataCount, dataMap);

                    // For graphical
                    // lvId = vdiDatas.get(vdiLvItems.get(j));
                    lvId = vdiInfoItems.get(j);
                    vdiInfoDatas.put(lvId, f);
                    // dataMap.put("value", "" + f);

                    // vdiDataCount++;
                }
                unpackOK = true;
                // }catch(Exception ex)
                // {
                // return false;
                // }
            }
        } catch (Exception e) {
            return false;
        }
        return unpackOK;
    }

    /* LV DATAS */
    private ArrayList<HashMap<Integer, String>> vdiTextDatas = new ArrayList<HashMap<Integer, String>>();
    private static HashMap<Integer, Integer> vdiDatas = new HashMap<Integer, Integer>();
    private ArrayList<Integer> vdiLvItems = new ArrayList<Integer>();

    public synchronized int getLvItem(int recvLength) {
        vdiLvItems.clear();
        int lvItem = 0;
        int id = 0;
        if (validChecksum(recvLength)) {
            for (int i = 3; i < recvLength - 1; i += 2) {
                HashMap<String, String> map = new HashMap<String, String>();
                id = (tmpBuffer[i] << 8) + tmpBuffer[i + 1];
                vdiLvItems.add(id);
                map.put("item", "" + id);
                data.add(map);

                // ////for text
                // HashMap<Integer,String> vdiValMap = new HashMap<Integer,
                // String>();
                // vdiValMap.put(id, "");
                // vdiTextDatas.add(vdiValMap);

                // for graphical
                vdiDatas.put(id, 0);
                lvItem++;
            }
            // adapter.notifyDataSetChanged();
        }

        return lvItem;
    }

    // NEED DEBUG
    @SuppressLint("NewApi")
    public synchronized boolean unpackVdiDatas(int recvLength) {
        boolean unpackOK = false;

        if (recvLength < 4)
            return false;

        // if(tmpBuffer[2] != (byte)0x6A)
        // return false;

        if (!((tmpBuffer[2] == (byte) 0x6A) || (tmpBuffer[2] == (byte) 0xFF)))
            return false;

        byte[] bytes = null;
        float f = 0.0f;
        int vdiDataCount = (recvLength - 4) / 4;

        int dataLength = ((tmpBuffer[0] << 8) + tmpBuffer[1]);
        dataLength += 2;
        if (dataLength != recvLength)
            return false;

        int lvId = 0;
        if (validChecksum(recvLength)) {
            // if(vdiDataCount <= 0)
            if (tmpBuffer[0] == 0x00 && tmpBuffer[1] == 0x02 && tmpBuffer[2] == 0x6A && tmpBuffer[3] == 0x6C) {
                synchronized (MonitorStates.LOCKER) {
                    MonitorStates.isIgOff = true;
                }

                // return false;
                return true;
            }

            // 遇到 0x00 0x02 0xFF 0x01
            if ((tmpBuffer[0] == 0x00) && (tmpBuffer[1] == 0x02) && (tmpBuffer[2] == (byte) 0xFF)
                    && (tmpBuffer[3] == 0x01))
                isAlreadyInit = false;

            try {
                // for (int i = 3,j=0; i < recvLength-1; i+=4,j++) {
                for (int i = 3, j = 0; j < vdiDataCount; i += 4, j++) {
                    bytes = Arrays.copyOfRange(tmpBuffer, i, i + 4);
                    f = ByteBuffer.wrap(bytes).order(ByteOrder.BIG_ENDIAN).getFloat();

                    // //For Text
                    // HashMap dataMap = data.get(vdiDataCount);
                    // setLvItemDisplayText(dataMap,f);
                    // vdiTextDatas.set(vdiDataCount, dataMap);

                    // For graphical
                    // lvId = vdiDatas.get(vdiLvItems.get(j));
                    int val = (int) f;
                    lvId = vdiLvItems.get(j);
                    vdiDatas.put(lvId, val);
                    if (lvId == 4 && val == 0) {
                        Log.d(CLASSNAME, "ID = " + lvId + " , val = " + val);
                    }
                    // dataMap.put("value", "" + f);

                    // vdiDataCount++;
                }
                unpackOK = true;
            } catch (Exception ex) {
                return false;
            }
        }
        return unpackOK;
    }

    // Build the GetAllLvValCmd Command
    public void buildGetAllLvValCmd() {
        int itemCount = vdiLvItems.size();
        //2016-11-15 Modify No Lv Data ID Problem.
        if(itemCount == 0)
        	vdiLvItems.add(0xFFFF);
        itemCount = vdiLvItems.size();
        
        int lengthHeaderLen = 2;
        int cmdLen = 1;
        int csLen = 1;
        int cmdSize = lengthHeaderLen + cmdLen + itemCount * 2 + csLen;
        VdiCommand.GetAllLvValCmd = new byte[cmdSize];
        VdiCommand.GetAllLvValCmd[0] = 0x00;
        VdiCommand.GetAllLvValCmd[1] = (byte) (VdiCommand.GetAllLvValCmd.length - 2);
        VdiCommand.GetAllLvValCmd[2] = 0x2A;

        for (int i = 4, j = 0; j < itemCount; i += 2, j++) {
            // Object []objArray = vdiDatas.get(j).keySet().toArray();
            // VdiCommand.GetAllLvValCmd[i] =
            // ((Integer)objArray[0]).byteValue();
            VdiCommand.GetAllLvValCmd[i] = vdiLvItems.get(j).byteValue();
        }
        byte checkSum = (byte) CommandFactory.computeChecksum(VdiCommand.GetAllLvValCmd,
                VdiCommand.GetAllLvValCmd.length);
        VdiCommand.GetAllLvValCmd[VdiCommand.GetAllLvValCmd.length - 1] = checkSum;

    }

    public void setLvItemDisplayText(HashMap<String, String> lvItemMap, float lvItemVal) {
        String id = "";
        id = (String) lvItemMap.get("item");
        if (id.equals("1")) {
            lvItemMap.put("item", "Vehicle Speed");
            lvItemMap.put("unit", "km/h");
        } else if (id.equals("2")) {
            lvItemMap.put("item", "Engine Speed");
            lvItemMap.put("unit", "rpm");
        } else if (id.equals("3")) {
            lvItemMap.put("item", "Park Switch");
        } else if (id.equals("4")) {
            lvItemMap.put("item", "Break Switch");
        } else if (id.equals("5")) {
            lvItemMap.put("item", "Cluch Switch");
        } else if (id.equals("6")) {
            String satus = "OFF";
            lvItemMap.put("item", "Turn Light Switch(L)");
        } else if (id.equals("7")) {
            lvItemMap.put("item", "Turn Light Switch(R)");
        } else if (id.equals("8")) {
            lvItemMap.put("item", "R Potion");
        } else if (id.equals("9")) {
            lvItemMap.put("item", "Seat Belt");
        } else if (id.equals("10")) {
            lvItemMap.put("item", "Door");
        }

        if ((id.equals("1") || id.equals("2") || id.equals("Vehicle Speed") || id.equals("Engine Speed")))
            lvItemMap.put("value", "" + lvItemVal);
        else {
            if (AppAttributes.CAR_TYPE == AppAttributes.CarType.PASSENGER_CAR) {
                if ((id.equals("6") || id.equals("7") || id.equals("Turn Light Switch(L)")
                        || id.equals("Turn Light Switch(R)")))
                    lvItemMap.put("value", (lvItemVal == 0.0) ? "ON" : "OFF");
                else
                    lvItemMap.put("value", (lvItemVal == 0.0) ? "OFF" : "ON");
            } else
                lvItemMap.put("value", (lvItemVal == 0.0) ? "OFF" : "ON");
        }

    }

    private static long randomTimer = 0;
    private Stopwatch testTimer = new Stopwatch();
    boolean firsttimeEnableRandomSetting = false;

    // 隨機取得範圍值
    private int getRandomTimer(int min, int max) {
        Random ran = new Random();
        return ran.nextInt((max - min) + 1) + min;
    }

    // Random Horn Act 設定
    private void RunRandomHornActSetting() {
        if (firsttimeEnableRandomSetting == false) {
            randomTimer = getRandomTimer(200, 500);
            firsttimeEnableRandomSetting = true;
        }
        if (testTimer.getElapsedTime().getElapsedRealtimeMillis() > randomTimer) {
            IsHornAct = true;
            randomTimer = getRandomTimer(200, 500);
            testTimer.reset();
        }
    }

    private boolean isInitedUSB = false;
    private int errorCount = 0;
    private static int targetCmdDefaultLength = 100;
    private static byte[] tmpBuffer = new byte[targetCmdDefaultLength];
    private static int tmpCount = 0;
    private static boolean isFirstSet = false;

    // send file thread
    private class Send_File_Thread extends Thread {
        private Handler mHandler;
        private FileInputStream instream;

        Send_File_Thread(Handler h, FileInputStream stream) {
            mHandler = h;
            instream = stream;
            this.setPriority(Thread.MAX_PRIORITY);
        }

        @Override
        public void run() {

            boolean isSentInitCmd = false;
            byte[] usbdata = new byte[64];
            int readcount = 0;
            int recvLength = 0;
            totalDataCount = 0;

            Message msg;
            start_time = System.currentTimeMillis();
            cal_time_1 = System.currentTimeMillis();

            // recvLength = sendAndReadData(FT311UARTInterface.InitCmd);
            // recvLength = sendAndReadData(FT311UARTInterface.GetAllLvIDCmd);
            // int lvItemCount = getLvItem(recvLength);
            // buildGetAllLvValCmd();

            // STATE MACHINE
            // COMMUNICATION MAIN
            Log.d(CLASSNAME, "" + data.size());
            // for (int i = 0; i < 100000; i++) {
            int i = 0;
            long startTime = 0;
            long endTime = 0;
            long startTotalTime = 0;
            long endTotalTime = 0;

            byte[] infoCmd = null;
            byte[] freezeDataCmd = null;
            while (true) {


                // if(uartInterface.hasUsbPermission() == false)
                // {
                // if(!exitDialog.isShowing())
                // {
                // runOnUiThread(new Runnable() {
                // public void run() {
                // exitDialog.show();
                // }
                // });
                //
                // }
                // }

                if (isAppPause == false && uartInterface.hasUsbPermission()) {
                    if (warningDialog.isShowing())
                        warningDialog.dismiss();

                    // if(exitDialog.isShowing())
                    // exitDialog.dismiss();

                    // ENABLE PIPE ERROR CHECK
                    pipeErrorCheck();

                    // if(isInitedUSB == false)
                    // {
                    // uartInterface.send115200InitCmd();
                    // isInitedUSB = true;
                    // }

                    // INITIAL COMMAND
                    if (isAlreadyInit == false) {
                        do {
                            recvLength = sendAndReadData(VdiCommand.InitCmd, commOK);
                            Log.d(CLASSNAME, "commOK : " + commOK);
                            if (recvLength >= 7) {
                                if (tmpBuffer[2] == 0x50)
                                    break;
                            }
                            try {
                                Thread.sleep(sendPeriodMillisec);
                            } catch (InterruptedException e) {
                                // TODO Auto-generated catch block
                                e.printStackTrace();
                            }
                        } while (isNeedResend);
                        isNeedResend = true;

                        do {
                            recvLength = sendAndReadData(VdiCommand.GetAllLvIDCmd, commOK);
                            if (tmpBuffer[2] == 0x63)
                                isNeedResend = false;
                            int lvItemCount = getLvItem(recvLength);
                            try {
                                Thread.sleep(sendPeriodMillisec);
                            } catch (InterruptedException e) {
                                // TODO Auto-generated catch block
                                e.printStackTrace();
                            }
                        } while (isNeedResend);

                        // if(isAlreadyGetLvCmd == false)
                        buildGetAllLvValCmd();

                        isNeedResend = true;

                        initInfoItems();
                        if (infoCmd == null) {
                            infoCmd = VdiCommand.getInfoDataCmd(vdiInfoItems);
                        }

                        if (AppAttributes.CAR_TYPE == CarType.TRUCK)
                            initFreezeItemsForTruck();
                        else
                            initFreezeItemsForSedan();

                        if (freezeDataCmd == null)
                            freezeDataCmd = VdiCommand.getFreezeDataCmd(vdiFreezeItems);

                        isAlreadyInit = true;
                        isAlreadyGetLvCmd = true;
                    }

                    // GET SETTING VALUE
                    // if(CurrentPage == PageName.Setting &&
                    // (MonitorStates.hasGotSettingValue == false))
                    if (MonitorStates.hasGotSettingValue == false) {
                        do {
                            recvLength = sendAndReadData(VdiCommand.readReferenceMemoryCmd, commOK);
                            if (tmpBuffer[2] == 0x62) {
                                settingValue = getSettingValueFromCmd(recvLength);
                                if (settingValue != null) {
                                    String bzcStr = settingValue.getContent("BZC");
                                    int bzc = 1;
                                    if (bzcStr != null)
                                        bzc = Integer.valueOf(bzcStr);
                                    bzc = bzc / (60 * 1000);
                                    SettingTable.BZC = bzc;
                                    isNeedResend = false;
                                    break;
                                }
                            }
                            try {
                                Thread.sleep(sendPeriodMillisec);
                            } catch (InterruptedException e) {
                                // TODO Auto-generated catch block
                                e.printStackTrace();
                            }
                        } while (isNeedResend);
                        isNeedResend = true;

                        if (settingValue != null)
                            SettingTable.setValue(settingValue);

                        try {
                            Log.d(CLASSNAME, BytesTool.bytesToHex(tmpBuffer, recvLength));
                        } catch (Exception ex) {
                            ex.printStackTrace();
                        }

                        final SettingTable settingTable = SettingTableFragment.getSettingTable();
                        if (settingTable != null) {
                            runOnUiThread(new Runnable() {
                                public void run() {
                                    settingTable.setUiDefaultValue(settingValue);
                                }
                            });
                        }
                        synchronized (MonitorStates.LOCKER) {
                            MonitorStates.hasGotSettingValue = true;
                        }
                    }

                    // SAVE SETTING VALUE
                    // if(CurrentPage == PageName.Setting &&
                    // MonitorStates.hasWriteRefMemoryRequest)
                    if (MonitorStates.hasWriteRefMemoryRequest) {
                        Log.d(CLASSNAME, "" + MainActivity.settingValue);
                        // byte []writeRefMemoryCmd =
                        // VdiCommand.getWriteRefMemoryCmd(settingValue);
                        byte[] writeRefMemoryCmd = VdiCommand.getWriteRefMemoryCmd();
                        do {
                            recvLength = sendAndReadData(writeRefMemoryCmd, commOK);
                            if (tmpBuffer[2] == 0x7B) 
                                isNeedResend = false;
                            try {
                                Thread.sleep(sendPeriodMillisec);
                            } catch (InterruptedException e) {
                                // TODO Auto-generated catch block
                                e.printStackTrace();
                            }
                        } while (isNeedResend);
                        isNeedResend = true;

                        synchronized (MonitorStates.LOCKER) {
                            MonitorStates.hasWriteRefMemoryRequest = false;
                        }
                    }

                    // SEND LIVE DATA
                    startTotalTime = System.currentTimeMillis();
                    Log.d(CLASSNAME, "LV Data , i = " + (++i));
                    startTime = System.currentTimeMillis();
                    do {
                        recvLength = sendAndReadData(VdiCommand.GetAllLvValCmd, commOK);
                        isNeedResend = (!unpackDatas(recvLength));
                        try {
                            Thread.sleep(sendPeriodMillisec);
                        } catch (InterruptedException e) {
                            // TODO Auto-generated catch block
                            e.printStackTrace();
                        }
                    } while (isNeedResend);
                    isNeedResend = true;

                    // unpackVdiDatas(recvLength);

                    // 當通訊錯誤時，彈出錯誤視窗，停止通訊
                    if (commOK == false) {
                        errorCount++;
                    }

                    if (errorCount >= 100) {
                        runOnUiThread(new Runnable() {
                            public void run() {
                                warningDialog.setMessage(getResources().getText(R.string.warningMessage));
                                warningDialog.setButton(DialogInterface.BUTTON_POSITIVE,
                                        getResources().getText(R.string.OK), new DialogInterface.OnClickListener() {

                                            @Override
                                            public void onClick(DialogInterface dialog, int which) {
                                                // TODO Auto-generated method
                                                // stub
                                                CloseApp();
                                            }
                                        });

                                if (!warningDialog.isShowing())
                                    warningDialog.show();
                                isAppPause = true;

                            }
                        });
                    }

                    endTime = System.currentTimeMillis();
                    Log.d(CLASSNAME, "sendAndReadData Time : " + (endTime - startTime) + " ms");

                    startTime = System.currentTimeMillis();

                    // IG OFF HANDLE
                    if (MonitorStates.isIgOff) {
                        // SEND FREEZE DATA ,GET RESULT
                        do {
                            recvLength = sendAndReadData(freezeDataCmd, commOK);
                            isNeedResend = (!unpackDatas(recvLength));
                            try {
                                Thread.sleep(sendPeriodMillisec);
                            } catch (InterruptedException e) {
                                // TODO Auto-generated catch block
                                e.printStackTrace();
                            }
                        } while (isNeedResend);
                        isNeedResend = true;

                        // unpackFreezeDatas(recvLength);

                        synchronized (MonitorStates.LOCKER) {
                            MonitorStates.hasFreezeDataRequest = false;
                        }
                        synchronized (MonitorStates.LOCKER) {
                            MonitorStates.isIgOff = false;
                        }

//                        if (MonitorStates.started) {
//                            // STORE DATA
////							MainActivity.gauges_fragment.setResultValue();
//                            MainActivity.gauges_fragment.storeValues();
//                        }
//
//                        do {
//                            recvLength = sendAndReadData(VdiCommand.exitCmd, commOK);
//                            isNeedResend = false;
//                            try {
//                                Thread.sleep(sendPeriodMillisec);
//                            } catch (InterruptedException e) {
//                                // TODO Auto-generated catch block
//                                e.printStackTrace();
//                            }
//                        } while (isNeedResend);
//                        isNeedResend = true;
                        
                        if (MonitorStates.started) {
							//SET ENGINE STALL
							setEngineStallConfigInfo(true);
							
							// STORE DATA
							if(AppAttributes.CAR_TYPE == CarType.TRUCK)
								MainActivity.gauges_fragment.storeValues();
						}
                        
                      //ENABLE RESUME FLAG ,FIRST TIME SAVE DB , OTHER TIMES UPDATE DB
						synchronized (MonitorStates.LOCKER) {
							MonitorStates.isResume = true;
						}

						//ENABLE TIMER 5 SECONDS
						boolean isNeedSendExitCmd = false;
						long igOffCloseTimestamp = 5000L;
						long igStartTime = System.currentTimeMillis();
						long igEndTime = 0;
						long diffTime = 0;
						Log.d(CLASSNAME, "IG OFF - START TIME : " + igStartTime);
						do {
							recvLength = sendAndReadData(VdiCommand.GetAllLvValCmd, commOK);
							if(recvLength > 4)
								break;
							isNeedResend = (!unpackDatas(recvLength));
							try {
								Thread.sleep(sendPeriodMillisec);
							} catch (InterruptedException e) {
								// TODO Auto-generated catch block
								e.printStackTrace();
							}
							igEndTime = System.currentTimeMillis();
							diffTime = igEndTime - igStartTime;
							Log.d(CLASSNAME,"diffTime : " + diffTime);
							if(diffTime >= igOffCloseTimestamp)
							{
								isNeedSendExitCmd = true;
								break;
							}

						} while (true);
						isNeedResend = true;
						igEndTime = System.currentTimeMillis();
						diffTime = igEndTime - igStartTime;
						Log.d(CLASSNAME,"End-diffTime : " + diffTime);
						
						if(isNeedSendExitCmd)
						{
							do {
								recvLength = sendAndReadData(VdiCommand.exitCmd, commOK);
								isNeedResend = false;
								try {
									Thread.sleep(sendPeriodMillisec);
								} catch (InterruptedException e) {
									// TODO Auto-generated catch block
									e.printStackTrace();
								}
							} while (isNeedResend);
							isNeedResend = true;
						}
                    }

                    // GET INFOMATION DATA
                    if (MonitorStates.hasInfoDataRequest ||
                            CurrentPage == PageName.Monitor_Main ||
                            ((CurrentPage == PageName.Result) && (results_fragment.getShowMode() == ShowMode.DYNAMIC_SHOW_MODE))) {
                        do {
                            recvLength = sendAndReadData(infoCmd, commOK);
                            isNeedResend = (!unpackDatas(recvLength));
                            try {
                                Thread.sleep(sendPeriodMillisec);
                            } catch (InterruptedException e) {
                                // TODO Auto-generated catch block
                                e.printStackTrace();
                            }
                        } while (isNeedResend);
                        isNeedResend = true;

                        // unpackInfoDatas(recvLength);

                    }

                    // SEND ERASE MEMORY CMD
                    if (MonitorStates.hasEraseMemoryRequest) {
                        do {
                            recvLength = sendAndReadData(VdiCommand.eraseMemoryCmd, commOK);
                            if (tmpBuffer[2] == 0x71)
                                isNeedResend = false;
                            try {
                                Thread.sleep(sendPeriodMillisec);
                            } catch (InterruptedException e) {
                                // TODO Auto-generated catch block
                                e.printStackTrace();
                            }
                        } while (isNeedResend);
                        isNeedResend = true;

                        synchronized (MonitorStates.LOCKER) {
                            MonitorStates.hasEraseMemoryRequest = false;
                        }
                    }

                    // SEND FREEZE DATA CMD
                    if (MonitorStates.hasFreezeDataRequest || ((CurrentPage == PageName.Result)
                            && (results_fragment.getShowMode() == ShowMode.DYNAMIC_SHOW_MODE))) {
                        do {
                            recvLength = sendAndReadData(freezeDataCmd, commOK);
                            isNeedResend = (!unpackDatas(recvLength));
                            try {
                                Thread.sleep(sendPeriodMillisec);
                            } catch (InterruptedException e) {
                                // TODO Auto-generated catch block
                                e.printStackTrace();
                            }
                        } while (isNeedResend);
                        isNeedResend = true;

                        // unpackFreezeDatas(recvLength);

                        synchronized (MonitorStates.LOCKER) {
                            MonitorStates.hasFreezeDataRequest = false;
                        }
                    }

                    // ALARM VOICE
                    // runAlarmSound();

                    // IDLE TIME SUMMATION
                    if (MonitorStates.started) {
                        if (vdiDatas.containsKey(104)) {
                            if (vdiDatas.get(104) != 0)
                                newIdleTimer.start();
                            else
                                newIdleTimer.pause();

                            newIdleTotoalTime = newIdleTimer.getTimeMillisecs();
                        }
                    }

                    // 隨機發送喇叭Act設定
                    // RunRandomHornActSetting();

                    // Horn 一直送
                    // IsHornAct = true;
                    // 發送喇叭Act
                    // Run Horn Act
                    try {
                        RunHornAct();
                    } catch (Exception e1) {
                        // TODO Auto-generated catch block
                        e1.printStackTrace();
                    }

                    // 發送方向燈Act
                    // #region Run Indicator Act
                    // Run Indicator Act
                    if (IsLeftIndicatorButtonOn) {
                        synchronized (ACT_INDICATOR_ACTIVE_LOCKER) {
                            // if (IsLeftIndicatorAct)
                            // if (btnLeftIndicator.Tag.ToString() == "ON")
                            if (btnLeftIndicator.isChecked()) {
                                //if (isBtnLeftIndicatorChecked) {
                                // 運行左邊方向燈ACT函數
                                RunLeftIndicatorAct();
                                // if (CommModuleWifiNew.VdiRespFlag == 0xFF)
                                // errRespTimes++;
                            } else {
                                if (IsLeftIndicatorCloseOK == false) {
                                    // 關閉左邊方向燈ACT函數
                                    CloseLeftIndicatorAct();
                                    // 當發送完關閉方向燈指令後，將IsLeftIndicatorCloseOK旗標改為TRUE，
                                    // 如此一來，關閉指令就只會發送一次
                                    IsLeftIndicatorCloseOK = true;
                                    // if (CommModuleWifiNew.VdiRespFlag ==
                                    // 0xFF)
                                    // errRespTimes++;
                                }
                                // IsLeftIndicatorButtonOn = false;
                            }
                        }

                    }

                    if (IsRightIndicatorButtonOn) {
                        synchronized (ACT_INDICATOR_ACTIVE_LOCKER) {
                            // if (btnRightIndicator.Tag.ToString() == "ON")
                            if (btnRightIndicator.isChecked()) {
                                //if (isBtnRightIndicatorChecked) {
                                // 運行右邊方向燈ACT函數
                                RunRightIndicatorAct();
                                // if (CommModuleWifiNew.VdiRespFlag == 0xFF)
                                // errRespTimes++;
                            } else {
                                if (IsRightIndicatorCloseOK == false) {
                                    // 關閉右邊方向燈ACT函數
                                    CloseRightIndicatorAct();
                                    // 當發送完關閉方向燈指令後，將IsLeftIndicatorCloseOK旗標改為TRUE，
                                    // 如此一來，關閉指令就只會發送一次
                                    IsRightIndicatorCloseOK = true;
                                    // if (CommModuleWifiNew.VdiRespFlag ==
                                    // 0xFF)
                                    // errRespTimes++;
                                }
                                // IsRightIndicatorButtonOn = false;
                            }
                        }
                    }
                    // #endregion

//					// #region 發送室內燈指令
//					if (btnRoomLamp != null) {
//						if (btnRoomLamp.isChecked()) {
//							RunRoomLampAct();
//						} else {
//							if (IsRoomLampCloseOK == false) {
//								CloseRoomLampAct();
//								IsRoomLampCloseOK = true;
//
//								// if (CommModuleWifiNew.VdiRespFlag == 0xFF)
//								// errRespTimes++;
//							}
//						}
//					}

                    if (isBtnRoomLampNull == false) {
                        if (isBtnRoomLampIsChecked == true) {
                            RunRoomLampAct();
                        } else {
                            if (IsRoomLampCloseOK == false) {
                                CloseRoomLampAct();
                                IsRoomLampCloseOK = true;

                                // if (CommModuleWifiNew.VdiRespFlag == 0xFF)
                                // errRespTimes++;
                            }
                        }
                    }


                    // #endregion

                    endTime = System.currentTimeMillis();
                    Log.d(CLASSNAME, "unpackTime : " + (endTime - startTime) + " ms");

                    // runOnUiThread(new Runnable() {
                    // public void run() {
                    //
                    // if(listView != null)
                    // {
                    // listView.setAdapter(adapter);
                    // listView.invalidate();
                    // }
                    //
                    // if(vdiDatas.containsKey(1))
                    // {
                    // gauge.setTargetValue(vdiDatas.get(1));
                    // }
                    //
                    // if(vdiDatas.containsKey(2))
                    // {
                    // rpmgauge.setTargetValue(vdiDatas.get(2));
                    // }
                    // }
                    // });

                    try {
                        Thread.sleep(20);
                    } catch (InterruptedException e) {
                        // TODO Auto-generated catch block
                        e.printStackTrace();
                        final StackTraceElement[] stack = e.getStackTrace();
                        final String message = e.getMessage();
                        String exMsg = message;
                        for (int index = 0; index < stack.length; index++) {
                            exMsg += stack[index].toString();
                            exMsg += "\r\n";
                        }
                        Log.d(CLASSNAME, exMsg);
                        break;
                    }
                    endTotalTime = System.currentTimeMillis();
                    Log.d(CLASSNAME, "TotalTime : " + (endTotalTime - startTotalTime) + " ms");
                }


            } // END WHILE

            // cal_time_2 = System.currentTimeMillis();
            // if((cal_time_2 - cal_time_1) > 200)
            // {
            // msg = mHandler.obtainMessage(UPDATE_SEND_DATA);
            // mHandler.sendMessage(msg);
            // cal_time_1 = cal_time_2;
            // }
            //
            // end_time = System.currentTimeMillis();
            // msg = mHandler.obtainMessage(UPDATE_SEND_DATA_DONE);
            // mHandler.sendMessage(msg);

            //
        }


    }// END SEND THREAD ; STATE MACHINE

    // PIPE ERROR MONITOR
    public void pipeErrorCheck() {
        Log.d(MainActivity.CLASSNAME, "SSS :" + uartInterface.getPipeErrorCount());
        Log.d(MainActivity.CLASSNAME, "MMM :" + MonitorStates.isSaveOkInIgOff);
        if (MonitorStates.started) {
            if ((uartInterface.getPipeErrorCount() > 100) && MonitorStates.isSaveOkInIgOff) {
                Log.d(CLASSNAME, "Force Close App.");
                CloseApp();
            }
        } else {
            if ((uartInterface.getPipeErrorCount() > 100)) {
                Log.d(CLASSNAME, "Force Close App.");
                CloseApp();
            }
        }

    }

    private synchronized void runAlarmSound() {
        if (CurrentPage == PageName.Monitor_Main) {
            // VSS WARNING
            if (vdiDatas.containsKey(100)) {
                if (vdiDatas.get(100) > 0) {
                    synchronized (MonitorStates.LOCKER) {
                        MonitorStates.isVssWarning = true;
                    }
                } else {
                    synchronized (MonitorStates.LOCKER) {
                        MonitorStates.isVssWarning = false;
                    }
                }
            }

            // RPM WARNING
            if (vdiDatas.containsKey(101)) {
                if (vdiDatas.get(101) > 0) {
                    synchronized (MonitorStates.LOCKER) {
                        MonitorStates.isRpmWarning = true;
                    }
                } else {
                    synchronized (MonitorStates.LOCKER) {
                        MonitorStates.isRpmWarning = false;
                    }
                }
            }

            // TPS WARNING
            if (vdiDatas.containsKey(102)) {
                if (vdiDatas.get(102) > 0) {
                    synchronized (MonitorStates.LOCKER) {
                        MonitorStates.isAccelWarning = true;
                    }
                } else {
                    synchronized (MonitorStates.LOCKER) {
                        MonitorStates.isAccelWarning = false;
                    }
                }
            }

            // IDLE WARNING
            if (vdiDatas.containsKey(103)) {
                if (vdiDatas.get(103) > 0) {
                    synchronized (MonitorStates.LOCKER) {
                        MonitorStates.isIdleWarning = true;
                    }
                } else {
                    synchronized (MonitorStates.LOCKER) {
                        MonitorStates.isIdleWarning = false;
                    }
                }
            }

            // PLAY SOUND
            if (MonitorStates.isBuzzerOn) {
                if (MonitorStates.isVssWarning || MonitorStates.isRpmWarning || MonitorStates.isAccelWarning
                        || MonitorStates.isIdleWarning) {
                    if (!mp3Player.isPlaying()) {
                    }
                    mp3Player.play();
                }
            }
        }
    }

    public SettingValue getSettingValueFromCmd(int recvLength) {

        if (tmpBuffer[2] != 0x62)
            return null;

        int loopTimes = (recvLength - 4) / 5;
        // settingValue = new SettingValue();
        settingValue = SettingTable.getSettingValue();
        ByteBuffer buffer = ByteBuffer.wrap(tmpBuffer).order(ByteOrder.BIG_ENDIAN);
        Short dataLength = buffer.getShort();
        Byte respCmd = buffer.get();
        byte id = 0;
        float ng1Val = 0;
        float ng2Val = 0;
        long longVal = 0;
        float floatVal = 0;
        final String floatFormat = "%.0f";
        for (int i = 0; i < loopTimes; i++) {
            id = buffer.get();
            switch (id) {
                case 1:
                    floatVal = buffer.getFloat();
                    settingValue.setContent("VG", String.format(floatFormat, floatVal));
                    break;

                case 2:
                    longVal = buffer.getInt();
                    if (longVal == -1)
                        longVal = 0xFFFFFFFF;
                    else
                        longVal /= 1000;
                    settingValue.setContent("VC", "" + longVal);
                    break;

                case 3:
                    floatVal = buffer.getFloat();
                    ng1Val = floatVal;
//                    floatVal = ((floatVal / VdiConstants.RPM_CONSTANT) * 100);
//                    settingValue.setContent("NG1", String.format(floatFormat, floatVal));
                    break;

                case 4:
                    floatVal = buffer.getFloat();
                    ng2Val = floatVal;
//                    floatVal = ((floatVal / VdiConstants.RPM_CONSTANT) * 100);
//                    settingValue.setContent("NG2", String.format(floatFormat, floatVal));
                    break;

                case 5:
                    floatVal = buffer.getFloat();
                    settingValue.setContent("VA", String.format(floatFormat, floatVal));
                    break;

                case 6:
                    longVal = buffer.getInt();
                    if (longVal == -1)
                        longVal = 0xFFFFFFFF;
                    else
                        longVal /= 1000;
                    settingValue.setContent("NC", "" + longVal);
                    break;

                case 7:
                    floatVal = buffer.getFloat();
                    settingValue.setContent("AG", String.format(floatFormat, floatVal));
                    break;

                case 8:
                    longVal = buffer.getInt();
                    if (longVal == -1)
                        longVal = 0xFFFFFFFF;
                    else
                        longVal /= (60 * 1000);
                    settingValue.setContent("IC", "" + longVal);
                    break;

                case 9:
                    longVal = buffer.getInt();
                    settingValue.setContent("BZ", "" + longVal);
                    break;

                case 10:
                    longVal = buffer.getInt();
                    settingValue.setContent("BZC", "" + longVal);
                    break;

                case 11:
                    floatVal = buffer.getFloat();
                    settingValue.setContent("Kf3", String.format("%.03f", floatVal));
                    break;

                case 12:
                    longVal = buffer.getInt();
                    settingValue.setContent("CO2", "" + longVal);
                    break;
                case 13:
                    longVal = buffer.getInt();
                    break;
                //NE
                case 14:
                    floatVal = buffer.getFloat();
                    settingValue.setContent("PP", String.format(floatFormat, floatVal));
                    VdiConstants.RPM_CONSTANT = Long.parseLong(settingValue.getContent("PP"));

                    ng1Val = ((ng1Val / VdiConstants.RPM_CONSTANT) * 100);
                    settingValue.setContent("NG1", String.format(floatFormat, ng1Val));

                    ng2Val = ((ng2Val / VdiConstants.RPM_CONSTANT) * 100);
                    settingValue.setContent("NG2", String.format(floatFormat, ng2Val));

                    break;
                default:
                    break;
            }
        } // end for

        return settingValue;

    }

    public void testSomething() {
        HashMap<String, String> map = null;

        if (isFirstSet == false) {
            map = new HashMap<String, String>();
            map.put("item", "rpm");
            map.put("value", "1000" + (++tmpCount));
            data.add(map);
            map = new HashMap<String, String>();
            map.put("item", "speed");
            map.put("value", "50" + (++tmpCount));
            data.add(map);
            isFirstSet = true;
        } else {
            map = new HashMap<String, String>();
            map.put("item", "rpm");
            map.put("value", "1000" + (++tmpCount));
            data.set(0, map);
            map = new HashMap<String, String>();
            map.put("item", "speed");
            map.put("value", "50" + (++tmpCount));
            data.set(1, map);

        }

        if (isFirstSet == false) {
            data.add(map);
            isFirstSet = true;
        } else {
            data.set(1, map);
        }

    }

	/* usb input data handler */
    // private class save_data_thread extends Thread
    // {
    // Handler mHandler;
    //
    // save_data_thread(Handler h) {
    // mHandler = h;
    // }
    //
    // @Override
    // public void run()
    // {
    // Message msg;
    // start_time = System.currentTimeMillis();
    // cal_time_1 = System.currentTimeMillis();
    // totalDataCount = 0;
    //
    // while (true == WriteFileThread_start)
    // {
    // try {
    // Thread.sleep(100);
    // } catch (InterruptedException e) {
    // }
    //
    // status = uartInterface.ReadData(4096, readBuffer,
    // actualNumBytes);
    //
    //// Log.e(">>@@", "actualNumBytes:" + actualNumBytes[0]);
    //
    // if (status == 0x00 && actualNumBytes[0] > 0)
    // {
    // totalDataCount += actualNumBytes[0];
    // try {
    // buf_save.write(readBuffer, 0, actualNumBytes[0]);
    // } catch (IOException e) {
    // e.printStackTrace();
    // }
    // }
    //
    // cal_time_2 = System.currentTimeMillis();
    // if((cal_time_2 - cal_time_1) > 200)
    // {
    // msg = mHandler.obtainMessage(UPDATE_READ_DATA);
    // mHandler.sendMessage(msg);
    // cal_time_1 = cal_time_2;
    // }
    // }
    //
    // end_time = System.currentTimeMillis();
    // msg = mHandler.obtainMessage(UPDATE_READ_DATA_DONE);
    // mHandler.sendMessage(msg);
    // }
    // }

    // public boolean sendAndReadData(byte [] cmd)
    // {
    // boolean sendAndReadDataOK = false;
    //
    // try{
    // int totolaReadLength = 0;
    // int targetLength = targetCmdDefaultLength;
    // int currentIndex = 0;
    // boolean isAlreadGetLength = false;
    // for (int i = 0; i < 1; i++) {
    // totolaReadLength = 0;
    // if(uartInterface.SendData(cmd.length, cmd) == 0x00)
    // {
    // long startTime = System.currentTimeMillis();
    // long endTime = startTime;
    // while(true)
    // {
    // actualNumBytes[0] = 0;
    // uartInterface.ReadData(readBuffer.length, readBuffer, actualNumBytes);
    // if(actualNumBytes[0] > 0)
    // {
    // for (int j = 0; j < actualNumBytes[0]; j++) {
    // tmpBuffer[currentIndex] = readBuffer[j];
    // currentIndex ++;
    // }
    //
    // if(currentIndex >=2 && isAlreadGetLength==false)
    // {
    // targetLength = (tmpBuffer[0] << 8 ) + tmpBuffer[1];
    // isAlreadGetLength = true;
    // }
    //
    // Log.d(ClassName,"i = " + i + " , Recv Datas : " +
    // bytesToHex(readBuffer,actualNumBytes[0]));
    // }
    // totolaReadLength += actualNumBytes[0];
    //
    //
    // //totolaReadLength over 255 ,break
    // if(totolaReadLength > 0xFF)
    // break;
    //
    // if(totolaReadLength >= (targetLength + 2))
    // {
    // //Log.d(ClassName,"i = " + i + ", Total Recv Datas : " +
    // bytesToHex(readBuffer,totolaReadLength));
    // Log.d(ClassName,"i = " + i + ", Total Recv Datas : " +
    // bytesToHex(tmpBuffer,totolaReadLength));
    // break;
    // }
    // endTime = System.currentTimeMillis();
    //
    // //Timeout mechanism ,if recvTime over 100ms
    // if((endTime - startTime) > 100)
    // break;
    // }
    // }
    // }//end for
    // sendAndReadDataOK = true;
    // }
    // catch(Exception ex)
    // {
    // return false;
    // }
    //
    // return sendAndReadDataOK;
    // }

    private Stopwatch commTimer = new Stopwatch();

    synchronized public int sendAndReadData(byte[] cmd, boolean commOK) {
        long costTime = 0;
        boolean sendAndReadDataOK = false;
        int targetLength = targetCmdDefaultLength;
        int totolaReadLength = 0;
        boolean isAlreadGetLength = false;
        int currentIndex = 0;
        String sendLogStr = "";
        String recvLogStr = "";
        try {
        	if(sendRecvLogContents.size() > SEND_RECV_LOG_MAX_COUNT)
        		sendRecvLogContents.clear();
        	
            for (int i = 0; i < 1; i++) {
                totolaReadLength = 0;
                costTime = 0;
                if (uartInterface.SendData(cmd.length, cmd) == 0x00) {
                	sendLogStr = "i = " + i + ", Send Datas : " + cmd.length + " bytes , "
                            + BytesTool.bytesToHex(cmd, cmd.length);
                    Log.d(CLASSNAME, sendLogStr);
                    sendRecvLogContents.add(getCurrentTimeFull() + " : " + sendLogStr + "\r\n");
                    // long startTime = System.currentTimeMillis();
                    // long endTime = startTime;
                    commTimer.reset();
                    while (true) {
                        if (isAppPause == true)
                            break;

                        actualNumBytes[0] = 0;
                        uartInterface.ReadData(readBuffer.length, readBuffer, actualNumBytes);
                        if (actualNumBytes[0] > 0) {
                            try {
                                for (int j = 0; j < actualNumBytes[0]; j++) {
                                    tmpBuffer[currentIndex] = readBuffer[j];
                                    currentIndex++;
                                }
                            } catch (Exception ex) {
                                return 0;
                            }

                            // if(currentIndex >=2 && isAlreadGetLength==false)
                            if (currentIndex >= 4 && isAlreadGetLength == false) {
                                targetLength = (tmpBuffer[0] << 8) + tmpBuffer[1];
                                isAlreadGetLength = true;
                            }

                            recvLogStr = "i = " + i + " , Recv Datas : " + actualNumBytes[0] + " bytes, "
                                    + BytesTool.bytesToHex(readBuffer, actualNumBytes[0]);
                            Log.d(CLASSNAME, recvLogStr);
                            sendRecvLogContents.add(getCurrentTimeFull() + " : " + recvLogStr + "\r\n");
                        }
                        totolaReadLength += actualNumBytes[0];

                        // totolaReadLength over 255 ,break
                        if (totolaReadLength > 0xFF)
                            break;

                        if (totolaReadLength >= (targetLength + 2)) {
                            // Log.d(ClassName,"i = " + i + ", Total Recv Datas
                            // : " + bytesToHex(readBuffer,totolaReadLength));
                            Log.d(CLASSNAME, "i = " + i + ", Total Recv Datas : " + totolaReadLength + " bytes , "
                                    + BytesTool.bytesToHex(tmpBuffer, totolaReadLength));
                            if (totolaReadLength > (targetLength + 2))
                                Log.d(CLASSNAME, "OVER RECV LENGTH : " + totolaReadLength);

                            // if(totolaReadLength > 0)
                            // isNeedResend = false;
                            // Log.d(CLASSNAME, "costTime : " + costTime + "
                            // ms");
                            // break;

                            if (totolaReadLength > 0) {
                                Log.d(CLASSNAME, "costTime : " + costTime + " ms");
                                break;
                            }
                        }
                        // endTime = System.currentTimeMillis();

                        // //Timeout mechanism ,if recvTime over 100ms
                        // if((endTime - startTime) > 100)
                        // break;
                        costTime = commTimer.getElapsedTime().getElapsedRealtimeMillis();
                        if (costTime > 100) {
                            Log.d(CLASSNAME, "costTime : " + costTime + " ms");
                            break;
                        }
                    }
                }
            } // end for
            sendAndReadDataOK = true;
        }catch(IOException ioEx)
        {
        	//2016-11-1 新增當USB SendData發生錯誤時，拋出IOExcetion
        	//當捕捉到IOException ， 儲存LOG，關閉APP
        	ioEx.printStackTrace();
        	logcatManager.Save(AppName, sendRecvLogContents);
        	CloseApp();
        }
        catch (Exception ex) {
            commOK = false;
            // return false;
            ex.printStackTrace();
        }
        // byte[] wordNeedSplit=tmpBuffer;
        // return wordNeedSplit;

        // return Arrays.copyOf(tmpBuffer, (targetLength + 2));
        commOK = true;
        return (targetLength + 2);
    }

    @Deprecated
    public boolean sendAndReadData(byte[] cmd, int loopTimes) {
        boolean sendAndReadDataOK = false;

        try {
            int totolaReadLength = 0;
            int targetLength = targetCmdDefaultLength;
            int currentIndex = 0;
            boolean isAlreadGetLength = false;
            for (int i = 0; i < loopTimes; i++) {
                totolaReadLength = 0;
                if (uartInterface.SendData(cmd.length, cmd) == 0x00) {
                    while (true) {
                        actualNumBytes[0] = 0;
                        uartInterface.ReadData(readBuffer.length, readBuffer, actualNumBytes);
                        if (actualNumBytes[0] > 0) {
                            for (int j = 0; j < actualNumBytes[0]; j++) {
                                tmpBuffer[currentIndex] = readBuffer[j];
                                currentIndex++;
                            }

                            if (currentIndex >= 2 && isAlreadGetLength == false) {
                                targetLength = (tmpBuffer[0] << 8) + tmpBuffer[1];
                                isAlreadGetLength = true;
                            }

                            Log.d(CLASSNAME, "i = " + i + " , Recv Datas : "
                                    + BytesTool.bytesToHex(readBuffer, actualNumBytes[0]));
                            System.out.println("" + CLASSNAME + "i = " + i + " , Recv Datas : "
                                    + BytesTool.bytesToHex(readBuffer, actualNumBytes[0]));
                        }
                        totolaReadLength += actualNumBytes[0];
                        if (totolaReadLength >= (targetLength + 2)) {
                            // Log.d(ClassName,"i = " + i + ", Total Recv Datas
                            // : " + bytesToHex(readBuffer,totolaReadLength));
                            Log.d(CLASSNAME, "i = " + i + ", Total Recv Datas : "
                                    + BytesTool.bytesToHex(tmpBuffer, totolaReadLength));
                            break;
                        }
                    }
                }
            } // end for
            sendAndReadDataOK = true;
        } catch (Exception ex) {
            // return false;
            ex.printStackTrace();
        }

        // byte[] totalWord=tmpBuffer.clone();
        return sendAndReadDataOK;
    }

    private void RunHornAct() throws Exception {

        // Run Horn Act
        synchronized (ACT_HORN_ACTIVE_LOCKER) {
            if (IsHornAct) {
                try {
                    //// 修改Act等到回覆完成 0x01繼續執行，如果回覆0x21 send again，
                    //// 且如果等待超過200 ms 跳出繼續執行
                    // wifi.vdiSendAndReadData(req, resp);
                    int recvLength = 0;
                    long startTime, endTime;
                    startTime = System.currentTimeMillis();
                    while (true) {
                        // wifi.vdiSendAndReadData(req, resp);
                        recvLength = sendAndReadData(VdiCommand.actHornCmd, commOK);
                        endTime = System.currentTimeMillis();
                        // if (resp.Data.Array[0] == 0x01 ||
                        // actSw.elapsed(TimeUnit.MILLISECONDS) >= 200)

                        if (recvLength >= 4) {
                            if (tmpBuffer[3] == 0x01) {
                                // actSw.stop();
                                break;
                            }
                        }

                        if ((endTime - startTime) >= 200)
                            break;
                    }

                    // 當RespFlag為0xFF，表示硬體不認識指令，繼續重送
                    if (tmpBuffer[2] == 0xFF)
                        IsHornAct = true;
                    else
                        IsHornAct = false;
                } catch (Exception ex) {
                    IsHornAct = false;
                    throw ex;
                }
            }
        }
    }

    private void RunLeftIndicatorAct() {
        synchronized (ACT_INDICATOR_ACTIVE_LOCKER) {
            // if (IsRightIndicatorAct)
            if (btnRightIndicator.getTag().toString().equals("ON")) {
                CloseRightIndicatorAct();
                IsRightIndicatorAct = false;
                btnRightIndicator.setTag("OFF");
                IsLeftIndicatorCloseOK = true;
            }

            try {

                //// 修改Act等到回覆完成 0x01繼續執行，如果回覆0x21 send again，
                //// 且如果等待超過200 ms 跳出繼續執行
                // wifi.vdiSendAndReadData(req, resp);
                int recvLength = 0;
                long startTime, endTime;
                startTime = System.currentTimeMillis();
                while (true) {
                    // wifi.vdiSendAndReadData(req, resp);
                    recvLength = sendAndReadData(VdiCommand.actLeftIndicatorOnCmd, commOK);
                    endTime = System.currentTimeMillis();
                    // if (resp.Data.Array[0] == 0x01 ||
                    // actSw.elapsed(TimeUnit.MILLISECONDS) >= 200)

                    if (recvLength >= 4) {
                        if (tmpBuffer[3] == 0x01) {
                            // actSw.stop();
                            break;
                        }
                    }

                    if ((endTime - startTime) >= 200)
                        break;
                }

                //// 當RespFlag為0xFF，表示硬體不認識指令，繼續重送
                // if (CommModuleWifiNew.VdiRespFlag == 0xFF)
                // IsLeftIndicatorAct = true;
                // else
                // IsLeftIndicatorAct = false;
            } catch (Exception ex) {
                IsLeftIndicatorAct = false;
                btnLeftIndicator.setTag("OFF");
                throw ex;
            }

        }
    }

    private void CloseLeftIndicatorAct() {
        try {
            int recvLength = 0;
            long startTime, endTime;
            startTime = System.currentTimeMillis();
            while (true) {
                // wifi.vdiSendAndReadData(req, resp);
                recvLength = sendAndReadData(VdiCommand.actLeftIndicatorOffCmd, commOK);
                endTime = System.currentTimeMillis();
                // if (resp.Data.Array[0] == 0x01 ||
                // actSw.elapsed(TimeUnit.MILLISECONDS) >= 200)

                if (recvLength >= 4) {
                    if (tmpBuffer[3] == 0x01) {
                        // actSw.stop();
                        break;
                    }
                }

                if ((endTime - startTime) >= 200)
                    break;
            }

            //// 當RespFlag為0xFF，表示硬體不認識指令，繼續重送
            // if (CommModuleWifiNew.VdiRespFlag == 0xFF)
            // IsLeftIndicatorAct = true;
            // else
            // IsLeftIndicatorAct = false;
        } catch (Exception ex) {
            IsLeftIndicatorAct = false;
            btnLeftIndicator.setTag("OFF");
            throw ex;
        }
    }

    String btnLeftIndicatorTagName = "";

    private void RunRightIndicatorAct() {
        synchronized (ACT_INDICATOR_ACTIVE_LOCKER) {
            // if (IsLeftIndicatorAct)
            if (btnLeftIndicator.getTag().toString().equals("ON")) {
                CloseLeftIndicatorAct();
                // IsLeftIndicatorAct = false;
                btnLeftIndicator.setTag("OFF");
                IsRightIndicatorCloseOK = true;
            }

            try {
                //// 修改Act等到回覆完成 0x01繼續執行，如果回覆0x21 send again，
                //// 且如果等待超過200 ms 跳出繼續執行
                // wifi.vdiSendAndReadData(req, resp);
                int recvLength = 0;
                long startTime, endTime;
                startTime = System.currentTimeMillis();
                while (true) {
                    // wifi.vdiSendAndReadData(req, resp);
                    recvLength = sendAndReadData(VdiCommand.actRightIndicatorOnCmd, commOK);
                    endTime = System.currentTimeMillis();
                    // if (resp.Data.Array[0] == 0x01 ||
                    // actSw.elapsed(TimeUnit.MILLISECONDS) >= 200)

                    if (recvLength >= 4) {
                        if (tmpBuffer[3] == 0x01) {
                            // actSw.stop();
                            break;
                        }
                    }

                    if ((endTime - startTime) >= 200)
                        break;
                }

                //// 當RespFlag為0xFF，表示硬體不認識指令，繼續重送
                // if (CommModuleWifiNew.VdiRespFlag == 0xFF)
                // IsRightIndicatorAct = true;
                // else
                // IsRightIndicatorAct = false;
            } catch (Exception ex) {
                IsRightIndicatorAct = false;
                btnRightIndicator.setTag("OFF");
                throw ex;
            }
        }
    }

    private void CloseRightIndicatorAct() {
        try {

            int recvLength = 0;
            long startTime, endTime;
            startTime = System.currentTimeMillis();
            while (true) {
                // wifi.vdiSendAndReadData(req, resp);
                recvLength = sendAndReadData(VdiCommand.actRightIndicatorOffCmd, commOK);
                endTime = System.currentTimeMillis();
                // if (resp.Data.Array[0] == 0x01 ||
                // actSw.elapsed(TimeUnit.MILLISECONDS) >= 200)

                if (recvLength >= 4) {
                    if (tmpBuffer[3] == 0x01) {
                        // actSw.stop();
                        break;
                    }
                }

                if ((endTime - startTime) >= 200)
                    break;
            }

            //// 當RespFlag為0xFF，表示硬體不認識指令，繼續重送
            // if (CommModuleWifiNew.VdiRespFlag == 0xFF)
            // IsLeftIndicatorAct = true;
            // else
            // IsLeftIndicatorAct = false;
        } catch (Exception ex) {
            IsLeftIndicatorAct = false;
            btnLeftIndicator.setTag("OFF");
            throw ex;
        }
    }

    private void RunRoomLampAct() {
        synchronized (ACT_INDICATOR_ACTIVE_LOCKER) {
            try {//// 修改Act等到回覆完成 0x01繼續執行，如果回覆0x21 send again，
                //// 且如果等待超過200 ms 跳出繼續執行
                // wifi.vdiSendAndReadData(req, resp);
                int recvLength = 0;
                long startTime, endTime;
                startTime = System.currentTimeMillis();
                while (true) {
                    // wifi.vdiSendAndReadData(req, resp);
                    recvLength = sendAndReadData(VdiCommand.actRoomLampOnCmd, commOK);
                    endTime = System.currentTimeMillis();
                    // if (resp.Data.Array[0] == 0x01 ||
                    // actSw.elapsed(TimeUnit.MILLISECONDS) >= 200)

                    if (recvLength >= 4) {
                        if (tmpBuffer[3] == 0x01) {
                            // actSw.stop();
                            break;
                        }
                    }

                    if ((endTime - startTime) >= 200)
                        break;
                }

            } catch (Exception ex) {
                throw ex;
            }
        }
    }

    private void CloseRoomLampAct() {
        try {
            //// 修改Act等到回覆完成 0x01繼續執行，如果回覆0x21 send again，
            //// 且如果等待超過200 ms 跳出繼續執行
            // wifi.vdiSendAndReadData(req, resp);
            int recvLength = 0;
            long startTime, endTime;
            startTime = System.currentTimeMillis();
            while (true) {
                // wifi.vdiSendAndReadData(req, resp);
                recvLength = sendAndReadData(VdiCommand.actRoomLampOffCmd, commOK);
                endTime = System.currentTimeMillis();
                // if (resp.Data.Array[0] == 0x01 ||
                // actSw.elapsed(TimeUnit.MILLISECONDS) >= 200)

                if (recvLength >= 4) {
                    if (tmpBuffer[3] == 0x01) {
                        // actSw.stop();
                        break;
                    }
                }

                if ((endTime - startTime) >= 200)
                    break;
            }

        } catch (Exception ex) {
            throw ex;
        }

    }

    public void CloseApp() {
        isAppExit = true;
        try {
            Thread.sleep(100);
        } catch (InterruptedException e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        }

        if (uartInterface != null)
            uartInterface.DestroyAccessory(true);

        try {
            Thread.sleep(100);
        } catch (InterruptedException e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        }
        android.os.Process.killProcess(android.os.Process.myPid());
    }

    // 獲得各項旗標 For Fragment
    public static Object getActHornActiveLocker() {
        return ACT_HORN_ACTIVE_LOCKER;
    }

    public static Object getLocker() {
        return LOCKER;
    }

    public static void setIsHornAct(boolean status) {
        IsHornAct = status;
    }

    public static boolean getIsHornAct() {
        return IsHornAct;
    }

    public static void setIsRoomLampCloseOK(boolean status) {
        IsRoomLampCloseOK = status;
    }

    public static boolean getIsRoomLampCloseOK() {
        return IsRoomLampCloseOK;
    }

    public static void setIsRightIndicatorCloseOK(boolean status) {
        IsRightIndicatorCloseOK = status;
    }

    public static boolean getIsRightIndicatorCloseOK() {
        return IsRightIndicatorCloseOK;
    }

    public static void setIsRightIndicatorButtonOn(boolean status) {
        IsRightIndicatorButtonOn = status;
    }

    public static boolean getIsRightIndicatorButtonOn() {
        return IsRightIndicatorButtonOn;
    }

    public static void setIsLeftIndicatorCloseOK(boolean status) {
        IsLeftIndicatorCloseOK = status;
    }

    public static boolean getIsLeftIndicatorCloseOK() {
        return IsLeftIndicatorCloseOK;
    }

    public static void setIsLeftIndicatorButtonOn(boolean status) {
        IsLeftIndicatorButtonOn = status;
    }

    public static boolean getIsLeftIndicatorButtonOn() {
        return IsLeftIndicatorButtonOn;
    }

    public static void setCanUsingIndicatorFlag(boolean status) {
        bCanUsingIndicator = status;
    }

    public static boolean getCanUsingIndicatorFlag() {
        return bCanUsingIndicator;
    }

    public static void setIsAppExit(boolean status) {
        isAppExit = status;
    }

    public static boolean GetIsAppExit() {
        return isAppExit;
    }

    public static MainActivity getInstance() {
        return (MainActivity) activity;
    }

    public static void setIndicatorOn(boolean status) {
        IndicatorOn = status;
    }

    public static boolean getIndicatorOn() {
        return IndicatorOn;
    }

    public static void setIsLeftIndicatorAct(boolean status) {
        IsLeftIndicatorAct = status;
    }

    public static boolean getIsLeftIndicatorAct() {
        return IsLeftIndicatorAct;
    }

    public static void setIsRightIndicatorAct(boolean status) {
        IsRightIndicatorAct = status;
    }

    public static boolean getIsRightIndicatorAct() {
        return IsRightIndicatorAct;
    }

    public static void setLeftFlashFlagOn(boolean status) {
        bLeftFlashOn = status;
    }

    public static boolean getLeftFlashFlagOn() {
        return bLeftFlashOn;
    }

    public static void setRightFlashFlagOn(boolean status) {
        bRightFlashOn = status;
    }

    public static boolean getRightFlashFlagOn() {
        return bRightFlashOn;
    }

    public static void setAllFlashFlagOn(boolean status) {
        bAllFlashOn = status;
    }

    public static boolean getAllFlashFlagOn() {
        return bAllFlashOn;
    }

    public static void setRoomLampCloseOK(boolean status) {
        IsRoomLampCloseOK = status;
    }

    public static boolean getRoomLampCloseOK() {
        return IsRoomLampCloseOK;
    }

    public static void setBtnRoomLampNullStatus(boolean isNull) {
        isBtnRoomLampNull = isNull;
    }

    public static boolean getBtnRoomLampNullStatus() {
        return isBtnRoomLampNull;
    }

    public static void setBtnRoomLampIsChecked(boolean isChecked) {
        isBtnRoomLampIsChecked = isChecked;
    }

    public static boolean getBtnRoomLampIsChecked() {
        return isBtnRoomLampIsChecked;
    }

    private static boolean isBtnLeftIndicatorChecked = false;

    public static void setBtnLeftIndicatorIsChecked(boolean isChecked) {
        isBtnLeftIndicatorChecked = isChecked;
    }

    public static boolean getBtnLeftIndicatorIsChecked() {
        return isBtnLeftIndicatorChecked;
    }

    private static boolean isBtnRightIndicatorChecked = false;

    public static void setBtnRightIndicatorIsChecked(boolean isChecked) {
        isBtnRightIndicatorChecked = isChecked;
    }

    public static boolean getBtnRightIndicatorIsChecked() {
        return isBtnRightIndicatorChecked;
    }

    @Override
    public void onBackPressed() {
        // TODO Auto-generated method stub
        // super.onBackPressed();
        if (AppAttributes.APP_MODE == AppMode.Optional) {
            if (CurrentPage == PageName.Monitor_Main || CurrentPage == PageName.Main) {
                Fragment instance = null;
                if (CurrentPage == PageName.Monitor_Main)
                    instance = gauges_fragment;
                else
                    instance = mainview_fragment;
                FragmentManager fragmentManager = getFragmentManager();
                FragmentTransaction fragmentTransaction = fragmentManager.beginTransaction();
                if (!MainActivity.optionFragment.isAdded())
                    fragmentTransaction.add(R.id.framelayout, MainActivity.optionFragment, "OPTION_FRAGMENT");
                fragmentTransaction.show(MainActivity.optionFragment);

                fragmentTransaction.hide(instance);
                fragmentTransaction.commit();
                MainActivity.CurrentPage = PageName.Option;
            }
        }
    }

    public static void setSettingValue(SettingValue value) {
        MainActivity.settingValue = value;
    }

    public static SettingValue getSettingValue() {
        return settingValue;
    }

    public static HashMap<Integer, Integer> getFreezeData() {
        return vdiFreezeDatas;
    }

    public static HashMap<Integer, Float> getInfoData() {
        return vdiInfoDatas;
    }

    public static HashMap<Integer, Integer> getVdiLvData() {
        return vdiDatas;
    }

    public static MP3Player getMP3Player() {
        return mp3Player;
    }

    //IDLE TIMER METHOD
    public static double getIdlePercent() {
        if (newIdleTimer == null)
            return 0;

        return newIdleTimer.getPercent();
    }

    public static void initIdleTimer() {
        if (newIdleTimer == null)
            return;

        newIdleTimer.init();
    }

    public static void startIdleTimer() {
        if (newIdleTimer == null)
            return;

        newIdleTimer.start();
    }

    public static void stopIdleTimer() {
        if (newIdleTimer == null)
            return;

        newIdleTimer.stop();
    }

    public static Date getStartDate() {
        return newIdleTimer.getStartDate();
    }

    public static String getStartDateStr() {
        return newIdleTimer.getStartDateStr();
    }

    public static String getEndDateStr() {
        return newIdleTimer.getEndDateStr();
    }

    public static String getIdleTime() {
        return newIdleTimer.getRemainTime();
    }

    public static String getTotalTime() {
        return newIdleTimer.getTotalTimeStr();
    }

}
