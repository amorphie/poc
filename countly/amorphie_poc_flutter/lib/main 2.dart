import 'dart:io';

import 'package:countly_flutter/countly_flutter.dart';
import 'package:flutter/material.dart';
import 'package:platform_device_id/platform_device_id.dart';

void main() {


  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  // This widget is the root of your application.
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Flutter Demo',
      theme: ThemeData(
        // This is the theme of your application.
        //
        // TRY THIS: Try running your application with "flutter run". You'll see
        // the application has a blue toolbar. Then, without quitting the app,
        // try changing the seedColor in the colorScheme below to Colors.green
        // and then invoke "hot reload" (save your changes or press the "hot
        // reload" button in a Flutter-supported IDE, or press "r" if you used
        // the command line to start the app).
        //
        // Notice that the counter didn't reset back to zero; the application
        // state is not lost during the reload. To reset the state, use hot
        // restart instead.
        //
        // This works for code too, not just values: Most code changes can be
        // tested with just a hot reload.
        colorScheme: ColorScheme.fromSeed(seedColor: Colors.deepPurple),
        useMaterial3: true,
      ),
      home: const MyHomePage(title: 'Flutter Demo Home Page'),
    );
  }
}

class MyHomePage extends StatefulWidget {
  const MyHomePage({super.key, required this.title});

  // This widget is the home page of your application. It is stateful, meaning
  // that it has a State object (defined below) that contains fields that affect
  // how it looks.

  // This class is the configuration for the state. It holds the values (in this
  // case the title) provided by the parent (in this case the App widget) and
  // used by the build method of the State. Fields in a Widget subclass are
  // always marked "final".

  final String title;

  @override
  State<MyHomePage> createState() => _MyHomePageState();
}

class _MyHomePageState extends State<MyHomePage> {
  int _counter = 0;
final bool _enableManualSession = false;

  @override
  void initState() {
    super.initState();

    String? deviceId = PlatformDeviceId.getDeviceId.toString();

  Countly.isInitialized().then((bool isInitialized) {
      if (!isInitialized) {
        Countly.pushTokenType(Countly.messagingMode[
            'TEST']!); // Set messaging mode for push notifications

        var crashSegment = {'Key': 'Value'};
        var userProperties = {
          'customProperty': 'custom Value',
          'username': 'USER_NAME',
          'email': 'USER_EMAIL'
        };

        Map<String, String> attributionValues = {};
        if (Platform.isIOS) {
          attributionValues[AttributionKey.IDFA] = 'IDFA';
        } else {
          attributionValues[AttributionKey.AdvertisingID] = 'AdvertisingID';
        }

        String campaignData =
            '{cid:"[PROVIDED_CAMPAIGN_ID]", cuid:"[PROVIDED_CAMPAIGN_USER_ID]"}';

        CountlyConfig config = CountlyConfig("http://localhost:8090", "173bd2fa9701e5a7040d4e497f76978543da94f3")
          ..enableCrashReporting() // Enable crash reporting to report unhandled crashes to Countly
          ..setRequiresConsent(
              true) // Set that consent should be required for features to work.
          ..setConsentEnabled([
            CountlyConsent.sessions,
            CountlyConsent.events,
            CountlyConsent.views,
            CountlyConsent.location,
            CountlyConsent.crashes,
            CountlyConsent.attribution,
            CountlyConsent.users,
            CountlyConsent.push,
            CountlyConsent.starRating,
            CountlyConsent.apm,
            CountlyConsent.feedback,
            CountlyConsent.remoteConfig
          ])
          ..setLocation(
              country_code: 'TR',
              city: 'Istanbul',
              ipAddress: '41.0082,28.9784',
              gpsCoordinates: '10.2.33.12') // Set user  location.
          ..setCustomCrashSegment(crashSegment)
          ..setUserProperties(userProperties)
          ..recordIndirectAttribution(attributionValues)
          ..recordDirectAttribution('countly', campaignData)
          ..setRemoteConfigAutomaticDownload(true, (error) {
            if (error != null) {
              print(error);
            }
          })
          ..remoteConfigRegisterGlobalCallback(
              (rResult, error, fullValueUpdate, downloadedValues) {
            if (error != null) {
              print(error);
            }
          }) // Set Automatic value download happens when the SDK is initiated or when the device ID is changed.
          ..setRecordAppStartTime(
              true) // Enable APM features, which includes the recording of app start time.
          ..setStarRatingTextMessage('Message for start rating dialog')
          ..setLoggingEnabled(true) // Enable countly internal debugging logs
          ..setParameterTamperingProtectionSalt(
              'salt') // Set the optional salt to be used for calculating the checksum of requested data which will be sent with each request
          ..setHttpPostForced(
              false); // Set to 'true' if you want HTTP POST to be used for all requests
        if (_enableManualSession) {
          config.enableManualSessionHandling();
        }
        Countly.initWithConfig(config).then((value) {
          Countly.appLoadingFinished();
          Countly.start();

          /// Push notifications settings
          /// Should be call after init
          Countly.onNotification((String notification) {
            print('The notification');
            print(notification);
          }); // Set callback to receive push notifications
          Countly
              .askForNotificationPermission(); // This method will ask for permission, enables push notification and send push token to countly server.;

          Countly
              .giveAllConsent(); // give consent for all features, should be call after init Countly.giveConsent(['events', 'views']); // give consent for some specific features, should be call after init.
        }); // Initialize the countly SDK.
      } else {
        print('Countly: Already initialized.');
      }
    });
  
  
  }

  void _incrementCounter() {
    setState(() {
      // This call to setState tells the Flutter framework that something has
      // changed in this State, which causes it to rerun the build method below
      // so that the display can reflect the updated values. If we changed
      // _counter without calling setState(), then the build method would not be
      // called again, and so nothing would appear to happen.
      _counter++;
    });
  }

  @override
  Widget build(BuildContext context) {
    // This method is rerun every time setState is called, for instance as done
    // by the _incrementCounter method above.
    //
    // The Flutter framework has been optimized to make rerunning build methods
    // fast, so that you can just rebuild anything that needs updating rather
    // than having to individually change instances of widgets.
    return Scaffold(
      appBar: AppBar(
        // TRY THIS: Try changing the color here to a specific color (to
        // Colors.amber, perhaps?) and trigger a hot reload to see the AppBar
        // change color while the other colors stay the same.
        backgroundColor: Theme.of(context).colorScheme.inversePrimary,
        // Here we take the value from the MyHomePage object that was created by
        // the App.build method, and use it to set our appbar title.
        title: Text(widget.title),
      ),
      body: Center(
        // Center is a layout widget. It takes a single child and positions it
        // in the middle of the parent.
        child: Column(
          // Column is also a layout widget. It takes a list of children and
          // arranges them vertically. By default, it sizes itself to fit its
          // children horizontally, and tries to be as tall as its parent.
          //
          // Column has various properties to control how it sizes itself and
          // how it positions its children. Here we use mainAxisAlignment to
          // center the children vertically; the main axis here is the vertical
          // axis because Columns are vertical (the cross axis would be
          // horizontal).
          //
          // TRY THIS: Invoke "debug painting" (choose the "Toggle Debug Paint"
          // action in the IDE, or press "p" in the console), to see the
          // wireframe for each widget.
          mainAxisAlignment: MainAxisAlignment.center,
          children: <Widget>[
            const Text(
              'You have pushed the button this many times:',
            ),
            Text(
              '$_counter',
              style: Theme.of(context).textTheme.headlineMedium,
            ),
          ],
        ),
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: _incrementCounter,
        tooltip: 'Increment',
        child: const Icon(Icons.add),
      ), // This trailing comma makes auto-formatting nicer for build methods.
    );
  }
}
