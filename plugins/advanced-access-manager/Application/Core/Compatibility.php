<?php

/**
 * ======================================================================
 * LICENSE: This file is subject to the terms and conditions defined in *
 * file 'license.txt', which is part of this source code package.       *
 * ======================================================================
 */

/**
 * Core compatibility with older versions
 * 
 * @package AAM
 * @author Vasyl Martyniuk <vasyl@vasyltech.com>
 * @todo   Remove Feb 2018
 */
class AAM_Core_Compatibility {
    
    /**
     * 
     */
    public static function initExtensions() {
        //block deprecated extensions from loading
        define('AAM_UTILITIES', '99');
        define('AAM_POST_FILTER', '99');
        define('AAM_REDIRECT', '99');
        define('AAM_CONTENT_TEASER', '99');
        define('AAM_LOGIN_REDIRECT', '99');
        //TODO - Remove this in Jul 2018
        
        //caching filter & action
        add_filter(
            'aam-read-cache-filter', 'AAM_Core_Compatibility::readCache', 10, 3
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
        $extensions = AAM_Core_API::getOption('aam-extensions', array(), 'site');
        
        if (empty($extensions)) {
            $extensions = AAM_Core_API::getOption('aam-extension-license', array(), 'site');
            if (!empty($extensions)) {
                $converted = array();
                
                foreach($extensions as $title => $license) {
                    $id = strtoupper(str_replace(' ', '_', $title));
                    if (defined($id)) { //include only installed
                        $converted[$id] = array(
                            'license' => $license,
                            'status'  => AAM_Extension_Repository::STATUS_INSTALLED
                        );
                    }
                }
                
                AAM_Core_API::updateOption('aam-extensions', $converted);
                AAM_Core_API::deleteOption('aam-extension-license');
            }
        }
        
        return $extensions;
    }
    
    /**
     * 
     * @return type
     */
    public static function getConfig() {
        $config = AAM_Core_API::getOption('aam-utilities', array());
        
        foreach(array_keys((is_array($config) ? $config : array())) as $option) {
            if (strpos($option, 'frontend.redirect') !== false) {
                self::convertConfigOption('redirect', $config, $option);
            } elseif (strpos($option, 'backend.redirect') !== false) {
                self::convertConfigOption('redirect', $config, $option);
            } elseif (strpos($option, 'login.redirect') !== false) {
                self::convertConfigOption('loginRedirect', $config, $option);
            } elseif (strpos($option, 'frontend.teaser') !== false) {
                self::convertConfigOption('teaser', $config, $option);
            }
        }
        
        return $config;
    }
    
    /**
     * 
     * @staticvar type $subject
     * @param type $oid
     * @param type &$config
     * @param type $option
     * 
     * @todo Legacy remove Jul 2018
     */
    protected static function convertConfigOption($oid, &$config, $option) {
        static $subject = null;
        
        if (is_null($subject)) {
            $subject = new AAM_Core_Subject_Default;
        }
        
        $subject->getObject($oid)->save($option, $config[$option]);
        unset($config[$option]);
        AAM_Core_API::updateOption('aam-utilities', $config);
    }

}