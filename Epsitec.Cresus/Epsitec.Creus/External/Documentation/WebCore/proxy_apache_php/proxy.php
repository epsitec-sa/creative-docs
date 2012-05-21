<?php

// URL from where get the content
$baseUrl = 'http://localhost:12345/';

// Need the session to have an ID
session_start();

function doRequest() {
  global $baseUrl;

  $url = $baseUrl . (isset($_GET['route']) ? $_GET['route'] : '');

  // Init
  $ch = curl_init();

  curl_setopt($ch, CURLOPT_URL, $url);

  if( "POST" == $_SERVER["REQUEST_METHOD"]) {
    curl_setopt($ch, CURLOPT_POST, 1);
    curl_setopt($ch, CURLOPT_POSTFIELDS, http_build_query($_POST)); // http_build_query() is required to handle arrays correctly
  }

  // Cookies from the request are stored on a file
  $cookieFile = realpath('.') . "/cookies/cookie_" . session_id() . ".txt";
  curl_setopt($ch, CURLOPT_COOKIEFILE, $cookieFile);
  curl_setopt($ch, CURLOPT_COOKIEJAR, $cookieFile);
  
  // Handle headers
  curl_setopt($ch, CURLOPT_HEADERFUNCTION	, "handleHeaders");

  // Directly print the content
  curl_exec($ch);
  
  curl_close($ch);
}

// This method checks the headers and redirects the first line,
// the one that says the HTTP status (200, 404, 500, ...)
function handleHeaders($ch, $data) {
  
  $pattern = '/^http/i';
  
  // Return the HTTP status code
  if(preg_match($pattern, $data)) {
    header($data);
  }

  return strlen($data);
}

doRequest();

?>