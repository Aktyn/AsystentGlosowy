### AsystentGłosowy
<div>Projekt stworzony na potrzeby zaliczenia przedmiotu na studiach.</div>

<div>Struktura projektu podzielona została na część frontendową i backend.</div>
 <div>Kod aplikacji webowej stworzony został w technologii ReactJS i odpowiedzialny jest między innymi za przetwarzanie mowy na tekst (Webkit Speech Recognition).</div>
<div>Kod asystenta jest programem stworzonym w technologi .NET. Odpowiada on za przetwarzanie i wykonywanie komend.</div>
<div>Komunikacja między frontendem a backendem odbywa się na zasadzie przesyłania komunikatów w formacie JSON poprzez WebSocket.</div>

#### Funkcjonalność
<div>
    ---
</div>

#### Uruchamianie
<ul>
    <li>
        <h5>Backend (polecenia wykonujemy z poziomu folderu <i>Asystent</i>)</h5>
        <i>Do poprawnego działania projektu wymagany jest klucz YouTube Data API v3. Można go wygenerować na <a href="https://console.cloud.google.com">https://console.cloud.google.com</a>.</i><br/><br/>
        <label>Tryb developerski:</label>
        <pre>dotnet watch run &ltYOUTUBE_API_KEY&gt</pre>
        <label>Skompilowanie projektu:</label>
        <pre>dotnet publish -c Release -r win-x64 --self-contained true</pre>
    </li>
    <li>
        <h5>Frontend (polecenia wykonujemy z poziomu folderu <i>frontend</i>)</h5>
        <label>Na początku należy pobrać i zainstalować zależności/paczki niezbędne do działania projektu</label>
        <pre>npm install</pre>
        <label>Tryb developerski:</label>
        <pre>npm run start</pre>
        <label>Skompilowanie projektu:</label>
        <pre>npm run build</pre>
    </li>
</ul>