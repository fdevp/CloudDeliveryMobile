# CloudDeliveryMobile
a mobile application for the CloudDelivery system. 

## Framework
applied the MvvmCross 5.6.3 framework that is the best solution for creating applications in the Xamarin Native technology.

## Shared library
with .Net Standard 1.4 support. Based on the IoC pattern, which is provided by the MvvmCross. Each of the injectable objects are declared in the App.cs file. 

#### Providers
provide basic device tools that can be shared, e.g. database, websockets handling, device info.

#### App database
Database is the sqlite db type. There is one table with a key-value structure. Keys store in the /Resources/DataKeys.resx file. Values are objects serialized to a JSON.

#### Session
Session is handled in the SessionProvider. It contains the HttpClient with set up authorization header and the base url properties. 

#### Backend communication
is based on the Refit library. Instances of the refit objects were created in App.cs. HttpClient with the session support are injected into them via constructors. Then, the refit objects are injected into the service objects.

#### Services
Services use injeceted refit objects to communicate with a backend server. I needed to share data between a few viewmodels and handle changes in the data structure. I decided to use thread safe events with a generic event model argument (/Models/ServiceEvent.cs). Each of the event from service layer passes in arguments an object that describes event's type and passes information about resources.

#### Websockets
applied a Signalr library that has a support for PCL. There is only one hub - for notifications. Hub - a service layer data exchange is handled by the thread-safe events. Either authorization token and events are set up in the root view models.

## Android
Application based primarly on fragments. There are only three activities: CarrierRoot, SalepointRoot, SignIn. The interaface was created on the base of the Material theme. The main interface element is a viewpager that contains three fragments. A fragments' management is handled by MvvmCross (except SalepointFloatingLabelFragment). 

#### Draggable recyclerview
achieved by creating a custom recyclerview adapter and viewholder.

#### Google Maps navigation interaction
was handled by intents.

#### Floating Widget
is created as a Bound Service. The floating widget service is bind to the main activity, and the activity-service communication is provided by the ServiceConnection object. A method that creates widget is placed in CarrierActivity, and is called from the active route fragment. A viewmodel is passed to the service in the active route fragment through a binder. Injecting a viewmodel into the service is safe, because the service, activity and viewmodel are placed in the same memory space. In the MvvmCross there is no support for binding in the services, so I decided to create my own controller that controls textviews and buttons in widget.

#### Geolocation Provider
I have tried to use the MvvmCross geolocation plugin, but unfortunetly there was a bug that prevented me to apply it. So, I have created a custom fused geolocation provider (Components/Geolocation/GeolocationProvider.cs).

#### Markers handling
was implemented in the markers' managers (Components/Map/). Markers are changing when a collection in the service layer changes. So,  the data flow is as follows: Service->ViewModel->View->Manager. I used MvxInteractions to handle it. When an event in service layer raises, the viewmodel handles it and sends information to the view with information,  which of the markers should be added/updated/deleted.

## Tests
Unfortunately, due to changes required by the client, I have got no time to implement tests. If I had time, I would use the same technologies that I used in the CloudDelivery project.

## ToDos
- TESTS!
- Endless scroll in the finished elements list  
- android version >= 6 permissions handling
- Profile fragment
- Filters
- "Do you want to close appliaction" modal when user press the back button
- Colors ValueConverters 
- Datetime picker for a required pickup time field
