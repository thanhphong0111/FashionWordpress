<?php

/**
 * ======================================================================
 * LICENSE: This file is subject to the terms and conditions defined in *
 * file 'license.txt', which is part of this source code package.       *
 * ======================================================================
 */

/**
 * Extension compatibility with older versions
 * 
 * @package AAM
 * @author Vasyl Martyniuk <vasyl@vasyltech.com>
 * @todo   Remove Feb 2018
 */
class AAM_Extension_Compatibility {
    
    /**
     * 
     */
    public static function init() {
        //block deprecated extensions from loading
        define('AAM_UTILITIES', '99');
        define('AAM_POST_FILTER', '99');
        define('AAM_REDIRECT', '99');
        define('AAM_CONTENT_TEASER', '99');
        define('AAM_LOGIN_REDIRECT', '99');
        //TODO - Remove this in Jul 2018
        
        //caching filter & action
        add_filter(
            'aam-read-cache-filter', 'AAM_Extension_Compatibility::readCache', 10, 3
        );
        
        //utilities option
        add_filter('aam-utility-property', 'AAM_Core_Config::get', 10, 2);
    }
    
    /**
     * 
     * @param type $value
     * @param type $option
     * @param type $subject
     * @return type
     */
    public static function readCache($value, $option, $subject) {
        return AAM_Core_Cache::get($option);
    }
    
    /**
     * 
     * @return type
     */
    public static function getExtensionList() {
        $extensions = AAM_Core_API::getOption('aam-extensions', array());
        
        if (empty($extensions)) {
            $extensions = AAM_Core_API::getOption('aam-extension-license', array());
            if (!empty($extensions)) {
                $converted = array();
                
                foreach($extensions as $title => $license) {
                    $converted[strtoupper(str_replace(' ', '_', $title))] = array(
                        'license' => $license,
                        'status'  => AAM_Extension_Repository::STATUS_INSTALLED
                    );
                }
                
                AAM_Core_API::updateOption('aam-extensions', $converted);
                AAM_Core_API::deleteOption('aam-extension-license');
            }
        }
        
        return $extensions;
    }

}