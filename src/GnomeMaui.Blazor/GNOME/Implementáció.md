# BlazorWebView GNOME Implementáció Terv

## Áttekintés

A BlazorWebView handler GNOME-ra való portolása, amely lehetővé teszi Blazor alkalmazások futtatását GNOME WebKit-ben.

## Referencia fájlok

### Főforrás

- `ext/maui/src/BlazorWebView/src/Maui/BlazorWebViewHandler.cs` - Közös handler kód
- `ext/maui/src/BlazorWebView/src/Maui/Android/BlazorWebViewHandler.Android.cs` - Android minta
- `ext/maui/src/BlazorWebView/src/Maui/iOS/BlazorWebViewHandler.iOS.cs` - iOS minta

### Működő GNOME példa

- `src/GnomeMaui.Blazor/GNOME/WebKitWebViewManager.MINTA.cs` - Teljes működő implementáció (standalone GirCore app)

### Implementálandó fájlok

- `src/GnomeMaui.Blazor/GNOME/BlazorWebViewHandler.GNOME.cs` - Jelenleg üres
- `src/GnomeMaui.Blazor/GNOME/GNOMEWebViewManager.cs` - Jelenleg üres

## Implementációs lépések

### 1. BlazorWebViewHandler.GNOME.cs

#### Alapstruktúra

```csharp
public partial class BlazorWebViewHandler : ViewHandler<IBlazorWebView, WebKit.WebView>
```

#### Szükséges mezők és property-k (Android/iOS alapján)

- [ ] `private GNOMEWebViewManager? _webviewManager` - WebView menedzser instance
- [ ] `internal GNOMEWebViewManager? WebviewManager => _webviewManager` - Publikus accessor
- [ ] `private ILogger? _logger` - Logger instance
- [ ] `internal ILogger Logger => _logger ??= Services!.GetService<ILogger<BlazorWebViewHandler>>() ?? NullLogger<BlazorWebViewHandler>.Instance`

#### CreatePlatformView() metódus

Minta: `WebKitWebViewManager.MINTA.cs` konstruktor

- [ ] WebKit.WebView létrehozása
- [ ] WebContext beállítása
- [ ] URI scheme regisztrálása (`app://`)
- [ ] UserContentManager létrehozása és beállítása
- [ ] JavaScript interop script hozzáadása (window.external objektum)
- [ ] Script message handler regisztrálása
- [ ] Navigation signal handler csatlakoztatása
- [ ] Developer tools engedélyezése (ha `DeveloperTools.Enabled`)
- [ ] BlazorWebViewInitializing/BlazorWebViewInitialized események kiváltása

**Kulcsfontosságú részletek a MINTA.cs-ből:**

```csharp
_webView.WebContext.RegisterUriScheme(Scheme, HandleUriScheme);
_userContentManager.AddScript(UserScript.New(...));
UserContentManager.ScriptMessageReceivedSignal.Connect(_userContentManager, WebviewInteropMessageReceived, true, "webview");
_userContentManager.RegisterScriptMessageHandler("webview", null);
```

#### DisconnectHandler() metódus

Android/iOS alapján:

- [ ] WebView leállítása (`platformView.StopLoading()` equivalent)
- [ ] WebViewManager disposal
  - [ ] `_webviewManager?.DisposeAsync()` hívás
  - [ ] Blocking/non-blocking disposal kezelése (IsBlockingDisposalEnabled kapcsoló)
  - [ ] Fire-and-forget pattern támogatása
- [ ] Signal handlerek lekapcsolása
- [ ] UserContentManager cleanup

#### StartWebViewCoreIfPossible() metódus

Android alapján:

- [ ] `RequiredStartupPropertiesSet` property implementálása
- [ ] Ellenőrzések (HostPage, Services, etc.)
- [ ] Content root directory meghatározása
- [ ] FileProvider létrehozása
- [ ] GNOMEWebViewManager inicializálása paraméterekkel:
  - PlatformView (WebKit.WebView)
  - Services
  - MauiDispatcher
  - FileProvider
  - JSComponents
  - contentRootDir
  - hostPageRelativePath
  - Logger
- [ ] StaticContentHotReloadManager csatolása
- [ ] BlazorWebViewInitializing/Initialized események
- [ ] RootComponents hozzáadása

### 2. GNOMEWebViewManager.cs

Minta: `WebKitWebViewManager.MINTA.cs`

#### Osztály definíció

```csharp
class GNOMEWebViewManager : WebViewManager, IAsyncDisposable
```

#### Konstruktor paraméterek (MINTA.cs alapján)

- [ ] `WebKit.WebView webView`
- [ ] `IServiceProvider services`
- [ ] `MauiDispatcher dispatcher`
- [ ] `IFileProvider fileProvider`
- [ ] `JSComponentConfigurationStore jsComponents`
- [ ] `string contentRootDir`
- [ ] `string hostPageRelativePath`
- [ ] `ILogger logger`

#### Base osztály inicializálás

- [ ] `BaseUri` definiálása (`app://127.0.0.1/` - már implementálva a HostAddressHelper-ben)
- [ ] WebViewManager konstruktor hívása a megfelelő paraméterekkel

#### HandleUriScheme() metódus

MINTA.cs `HandleUriScheme()` alapján:

- [ ] Scheme validálás
- [ ] URI parsing
- [ ] Static file vs Blazor route detektálás
- [ ] Content type meghatározása (MimeTypes.GetMimeType)
- [ ] TryGetResponseContent() hívás
- [ ] MemoryInputStream létrehozása GLib.Bytes-ból
- [ ] URISchemeRequest.Finish() hívás

**Fontos különbségek a MINTA.cs-hez képest:**

- A MINTA 127.0.0.1 helyett localhost-ot használ (BaseUri)
- Frissíteni kell 127.0.0.1-re (HostAddressHelper már ezt adja vissza)

#### NavigateCore() metódus

- [ ] `_webView.LoadUri(absoluteUri.ToString())` implementálása

#### SendMessage() metódus

- [ ] JavaScript értékelés WebView-ben
- [ ] `__dispatchMessageCallback()` hívása
- [ ] JavaScript string encoding (HttpUtility.JavaScriptStringEncode)

#### Message fogadás

- [ ] WebviewInteropMessageReceived signal handler
- [ ] MessageReceived() hívása a base osztályban

#### Navigation signal handler

- [ ] LinkClicked navigation type detektálása
- [ ] External browser indítása külső linkekhez
- [ ] Process.Start() használata Linux alatt

#### DisposeAsyncCore() metódus

- [ ] Base osztály dispose hívása
- [ ] Signal handlerek lekapcsolása
- [ ] UserContentManager dispose

### 3. StaticContentProvider.cs

Már létező fájl, ellenőrizni kell:

- [ ] Megfelelően működik-e a MINTA.cs-sel
- [ ] Szükséges-e módosítás a MAUI integrációhoz

### 4. Platformfüggőségek és konfigurációk

#### Project fájl (GnomeMaui.Blazor.csproj)

- [ ] Helyes fájl hivatkozások
- [ ] Condition="'$(TargetPlatform)' == 'GNOME'" használata
- [ ] BlazorWebView forrás fájlok linkje

#### Használt API-k GirCore-ból

MINTA.cs alapján:

- [x] `WebKit.WebView` - létrehozás és konfiguráció
- [x] `WebKit.Settings` - beállítások
- [x] `WebKit.UserContentManager` - script injection
- [x] `WebKit.UserScript` - JavaScript kód
- [x] `WebKit.URISchemeRequest` - custom scheme handling
- [x] `Gio.MemoryInputStream` - content streaming
- [x] `GLib.Bytes` - byte adatok kezelése
- [ ] Navigation signal API - ellenőrizni a pontos API-t

### 5. Tesztelés és debug

#### Debug kimenet

A MINTA.cs-ben használt debug üzenetek:

- [ ] URI fetch logging
- [ ] Request path logging
- [ ] Scheme validation errors

#### Példa alkalmazás

- [ ] samples/MauiTest1 vagy új Blazor példa
- [ ] Működik-e a navigation
- [ ] JavaScript interop működik-e
- [ ] Static file-ok betöltődnek-e

## Implementációs sorrend

1. **GNOMEWebViewManager.cs** - teljes implementáció a MINTA.cs alapján
   - Ez a "core" logika, ami működik standalone környezetben

2. **BlazorWebViewHandler.GNOME.cs** - MAUI integráció
   - CreatePlatformView() - WebView létrehozás és konfiguráció
   - StartWebViewCoreIfPossible() - GNOMEWebViewManager inicializálás
   - DisconnectHandler() - cleanup

3. **Tesztelés** - samples alkalmazás
   - Egyszerű Blazor oldal betöltése
   - Navigation tesztelése
   - JavaScript interop tesztelése

## Kulcskülönbségek a platformok között

### Android

- WebSettings objektum használata
- WebViewClient custom osztály
- Asset-alapú file serving

### iOS

- WKWebViewConfiguration
- WKUserScript használata
- Custom URL scheme handler (app://)
- NSUrlSchemeHandler protocol implementáció

### GNOME (MINTA.cs alapján)

- WebContext.RegisterUriScheme()
- UserContentManager + UserScript
- URISchemeRequest handling
- Signal-based event system
- Gio.MemoryInputStream használata

## Lehetséges problémák és megoldások

### 1. Signal API eltérések

**Probléma:** A MINTA.cs Internal API-kat használ
**Megoldás:** Ellenőrizni a GirCore Public API-t, hogy vannak-e megfelelő wrapperek

### 2. Memory management

**Probléma:** GLib/GTK objektumok disposal-ja
**Megoldás:** IAsyncDisposable megfelelő implementálása, using blokkok

### 3. Thread safety

**Probléma:** WebView API-k thread-sensitive lehetnek
**Megoldás:** MauiDispatcher használata minden WebView művelethez

### 4. Content loading

**Probléma:** Static content és Blazor route megkülönböztetése
**Megoldás:** MINTA.cs logikája - MIME type alapú detektálás

## Jegyzetek

- A MINTA.cs már működő kód, amely GirCore-on fut közvetlenül
- A fő feladat a MAUI integrációs réteg (Handler) implementálása
- A GNOMEWebViewManager nagyrészt átemelhető a MINTA.cs-ből
- Figyelni kell a BaseUri-ra: 127.0.0.1 (már beállítva a HostAddressHelper-ben)
