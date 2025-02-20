<?php

$appName = 'appName';
$appBundle = 'appBundle';

$secretKey = 'WZTM6UCTDN66CJV1';


if($secretKey == $_GET['{secretKey}']) {
    echo 'Привет я приложение '. $appName .' моя ссылка на гугл плей https://play.google.com/store/apps/details?id='. $appBundle ;
}