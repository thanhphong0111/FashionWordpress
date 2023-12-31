<?php

/**
 * ======================================================================
 * LICENSE: This file is subject to the terms and conditions defined in *
 * file 'license.txt', which is part of this source code package.       *
 * ======================================================================
 */

/**
 * Content teaser manager
 * 
 * @package AAM
 * @author Vasyl Martyniuk <vasyl@vasyltech.com>
 */
class AAM_Backend_Feature_Teaser extends AAM_Backend_Feature_Abstract {
    
    /**
     * 
     */
    public function save() {
        $param = AAM_Core_Request::post('param');
        $value = AAM_Core_Request::post('value');
        
        AAM_Backend_View::getSubject()->getObject('teaser')->save($param, $value);
        
        return json_encode(array('status' => 'success'));
    }
    
    /**
     * 
     * @return type
     */
    public function reset() {
        $subject = AAM_Backend_View::getSubject();
        $subject->getObject('teaser')->reset();
        
        return json_encode(array('status' => 'success')); 
    }
    
    /**
     * 
     * @return type
     */
    public function isDefault() {
        return (AAM_Backend_View::getSubject()->getUID() == 'default');
    }
    
    /**
     * Check inheritance status
     * 
     * Check if teaser settings are overwritten
     * 
     * @return boolean
     * 
     * @access protected
     */
    protected function isOverwritten() {
        $object = AAM_Backend_View::getSubject()->getObject('teaser');
        
        return $object->isOverwritten();
    }
    
    /**
     * 
     * @param type $option
     * @return type
     */
    public function getOption($option, $default = null) {
        $object = AAM_Backend_View::getSubject()->getObject('teaser');
        $value  = $object->get($option);
        
        return (!is_null($value) ? $value : $default);
    }
    
    /**
     * @inheritdoc
     */
    public static function getAccessOption() {
        return 'feature.teaser.capability';
    }
    
    /**
     * @inheritdoc
     */
    public static function getTemplate() {
        return 'object/teaser.phtml';
    }
    
    /**
     * Register Contact/Hire feature
     * 
     * @return void
     * 
     * @access public
     */
    public static function register() {
        $cap = AAM_Core_Config::get(self::getAccessOption(), 'administrator');
        
        AAM_Backend_Feature::registerFeature((object) array(
            'uid'        => 'teaser',
            'position'   => 40,
            'title'      => __('Content Teaser', AAM_KEY),
            'capability' => $cap,
            'subjects'   => array(
                'AAM_Core_Subject_Role', 
                'AAM_Core_Subject_User', 
                'AAM_Core_Subject_Visitor',
                'AAM_Core_Subject_Default'
            ),
            'view'       => __CLASS__
        ));
    }

}