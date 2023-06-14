<?php

/**
 * ======================================================================
 * LICENSE: This file is subject to the terms and conditions defined in *
 * file 'license.txt', which is part of this source code package.       *
 * ======================================================================
 */

return array(
    array(
        'title'       => 'AAM Plus Package',
        'id'          => 'AAM_PLUS_PACKAGE',
        'type'        => 'commercial',
        'price'       => '$30',
        'currency'    => 'USD',
        'description' => 'Our best selling extension that allows you to manage access to unlimited number of posts, pages or custom post types and define default access to ALL posts, pages, custom post types, categories or custom taxonomies. <a href="https://aamplugin.com/help/aam-plus-package-extension" target="_blank">Read more.</a>',
        'storeURL'    => 'https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=FGAHULDEFZV4U',
        'version'     => (defined('AAM_PLUS_PACKAGE') ? constant('AAM_PLUS_PACKAGE') : null)
    ),
    array(
        'title'       => 'AAM IP Check',
        'id'          => 'AAM_IP_CHECK',
        'type'        => 'commercial',
        'price'       => '$10',
        'currency'    => 'USD',
        'new'         => true,
        'description' => 'Manage access to your website based on the visitor geo-location, refered host or IP address. <a href="https://aamplugin.com/help/aam-ip-check-extension" target="_blank">Read more.</a>',
        'storeURL'    => 'https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=R5QYSA9ZUA2E4',
        'version'     => (defined('AAM_IP_CHECK') ? constant('AAM_IP_CHECK') : null)
    ),
    array(
        'title'       => 'AAM User Activity',
        'id'          => 'AAM_USER_ACTIVITY',
        'type'        => 'commercial',
        'price'       => '$10',
        'currency'    => 'USD',
        'new'         => true,
        'description' => 'Track any kind of user or visitor activity on your website. <a href="https://aamplugin.com/help/how-to-track-any-wordpress-user-activity" target="_blank">Read more.</a>',
        'storeURL'    => 'https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=WUZ7XBWHDNWS2',
        'version'     => (defined('AAM_USER_ACTIVITY') ? constant('AAM_USER_ACTIVITY') : null)
    ),
    array(
        'title'       => 'AAM Role Hierarchy',
        'id'          => 'AAM_ROLE_HIERARCHY',
        'type'        => 'commercial',
        'price'       => '$15',
        'currency'    => 'USD',
        'description' => 'Create complex role hierarchy and automatically inherit access settings from parent roles. <a href="https://aamplugin.com/help/aam-role-hierarchy-extension" target="_blank">Read more.</a>',
        'storeURL'    => 'https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=K8DMZ66SAW8VG',
        'version'     => (defined('AAM_ROLE_HIERARCHY') ? constant('AAM_ROLE_HIERARCHY') : null)
    ),
    array(
        'title'       => 'AAM Role Filter',
        'id'          => 'AAM_ROLE_FILTER',
        'type'        => 'commercial',
        'price'       => '$5',
        'currency'    => 'USD',
        'description' => 'Based on user levels, restrict access to manage list of roles and users that have higher user level. <a href="https://aamplugin.com/help/aam-role-filter-extension" target="_blank">Read more.</a>',
        'storeURL'    => 'https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=G9V4BT3T8WJSN',
        'version'     => (defined('AAM_ROLE_FILTER') ? constant('AAM_ROLE_FILTER') : null)
    ),
    array(
        'title'       => 'AAM Payment',
        'id'          => 'AAM_PAYMENT',
        'type'        => 'commercial',
        'price'       => '$20',
        'new'         => true,
        'currency'    => 'USD',
        'description' => 'Start selling access to your posts, categories or user levels. <a href="https://aamplugin.com/help/aam-payment-extension" target="_blank">Read more.</a>',
        'storeURL'    => 'https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=9ZRU8E7JBNF2W',
        'version'     => (defined('AAM_PAYMENT') ? constant('AAM_PAYMENT') : null)
    ),
     array(
        'title'       => 'AAM Complete Package',
        'id'          => 'AAM_COMPLETE_PACKAGE',
        'type'        => 'commercial',
        'price'       => '$70',
        'currency'    => 'USD',
        'description' => 'Get list of all available premium extensions in one package. Any additional premium extensions in the future will be included in this package. As of today, you already are saving $20 USD.',
        'storeURL'    => 'https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=THJWEJR3URR8L',
        'version'     => (defined('AAM_COMPLETE_PACKAGE') ? constant('AAM_COMPLETE_PACKAGE') : null)
    ),
    array(
        'title'       => 'AAM Multisite',
        'id'          => 'AAM_MULTISITE',
        'type'        => 'GNU',
        'license'     => 'AAMMULTISITE',
        'description' => 'Convenient way to navigate between different sites in the Network Admin Panel.',
        'version'     => (defined('AAM_MULTISITE') ? constant('AAM_MULTISITE') : null)
    ),
    array(
        'title'       => 'AAM ConfigPress',
        'id'          => 'AAM_CONFIGPRESS',
        'type'        => 'GNU',
        'license'     => 'AAMCONFIGPRESS',
        'description' => 'Extension to manage AAM core functionality with advanced configuration settings.',
        'version'     => (defined('AAM_CONFIGPRESS') ? constant('AAM_CONFIGPRESS') : null)
    )
);