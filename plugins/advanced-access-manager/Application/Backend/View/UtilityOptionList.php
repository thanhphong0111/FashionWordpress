<?php

/**
 * ======================================================================
 * LICENSE: This file is subject to the terms and conditions defined in *
 * file 'license.txt', which is part of this source code package.       *
 * ======================================================================
 */

return array(
    'manage-capability' => array(
        'title' => __('Edit/Delete Capabilities', AAM_KEY),
        'descr' => AAM_Backend_View_Helper::preparePhrase('[Please note!] For experienced users only. Allow to edit or delete capabilities', 'b'),
        'value' => AAM_Core_Config::get('manage-capability', false)
    ),
    'backend-access-control' => array(
        'title' => __('Backend Access Control', AAM_KEY),
        'descr' => __('Allow AAM to manage access to backend resources like backend menu, categories or posts.', AAM_KEY),
        'value' => AAM_Core_Config::get('backend-access-control', true),
    ),
    'frontend-access-control' => array(
        'title' => __('Frontend Access Control', AAM_KEY),
        'descr' => __('Allow AAM to manage access to frontend resources like pages, categories or posts.', AAM_KEY),
        'value' => AAM_Core_Config::get('frontend-access-control', true),
    ),
    'media-access-control' => array(
        'title' => __('Media Files Access Control', AAM_KEY),
        'descr' => AAM_Backend_View_Helper::preparePhrase('Allow AAM to manage a physically access to all media files located in the [uploads] folder.', 'strong'),
        'value' => AAM_Core_Config::get('media-access-control', false),
    ),
    'cache-auto-clear' => array(
        'title' => __('Clear cache automatically', AAM_KEY),
        'descr' => __('Clear AAM cache automatically during post saving or when any category is updated or deleted', AAM_KEY),
        'value' => AAM_Core_Config::get('cache-auto-clear', true),
    ),
    'large-post-number' => array(
        'title' => __('Enhance post filtering', AAM_KEY),
        'descr' => AAM_Backend_View_Helper::preparePhrase('[Warning!] This may significantly reduce your website load with large amount of posts and categories until AAM caches results. Modify database query to exclude posts that have LIST or LIST TO OTHERS options checked.', 'b'),
        'value' => AAM_Core_Config::get('large-post-number', false),
    )
);