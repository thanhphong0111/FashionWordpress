<?php

/**
 * ======================================================================
 * LICENSE: This file is subject to the terms and conditions defined in *
 * file 'license.txt', which is part of this source code package.       *
 * ======================================================================
 */

/**
 * Post object
 * 
 * @package AAM
 * @author Vasyl Martyniuk <vasyl@vasyltech.com>
 */
class AAM_Core_Object_Post extends AAM_Core_Object {

    /**
     * Post object
     * 
     * @var WP_Post
     * 
     * @access private
     */
    private $_post;
    
    /**
     * Constructor
     *
     * @param AAM_Core_Subject $subject
     * @param WP_Post|Int      $post
     *
     * @return void
     *
     * @access public
     */
    public function __construct(AAM_Core_Subject $subject, $post) {
        parent::__construct($subject);

        //make sure that we are dealing with WP_Post object
        if (is_a($post, 'WP_Post')) {
            $this->setPost($post);
        } elseif (intval($post)) {
            $this->setPost(get_post($post));
        }

        if ($this->getPost()) {
            $this->read();
        }
    }
    
    /**
     * 
     * @param type $name
     * @return type
     */
    public function __get($name) {
        $post = $this->getPost();
        
        return (property_exists($post, $name) ? $post->$name : null);
    }

    /**
     * Read the Post AAM Metadata
     *
     * Get all settings related to specified post.
     *
     * @return void
     *
     * @access public
     */
    public function read() {
        $subject = $this->getSubject();
        $opname  = $this->getOptionName();
        $chname  = $opname . '|' . $this->getPost()->ID;
        
        //read cache first
        $option = AAM_Core_Cache::get($chname);
        
        if ($option === false) { //if false, then the cache is empty but exist
            $option = array();
        } else {
            //Cache is empty. Get post access for current subject (user or role)
            if (empty($option)) { //no cache for this element
                $option = get_post_meta($this->getPost()->ID, $opname, true);
                $this->setOverwritten(!empty($option));
            }
            
            //try to inherit from terms or default settings - AAM Plus Package or any
            //other extension that use this filter
            if (empty($option)) {
                $option = apply_filters('aam-post-access-filter', $option, $this);
            }
            
            //No settings for a post. Try to inherit from the parent
            if (empty($option)) {
                $option = $subject->inheritFromParent('post', $this->getPost()->ID);
            }
            
            if (empty($option)) {
                $option = $subject->inheritFromDefault('post', $this->getPost()->ID);
            }
        }
        
        $this->setOption($option);
        
        //if result is empty, simply cache the false to speed-up
        AAM_Core_Cache::set($subject, $chname, (empty($option) ? false : $option));
    }
    
    /**
     * Save options
     * 
     * @return boolean
     * 
     * @access public
     */
    public function save($property, $checked) {
        $option = $this->getOption();
        
        $option[$property] = $checked;
        
        $result = update_post_meta(
                $this->getPost()->ID, $this->getOptionName(), $option
        );
        
        if ($result) {
            $this->setOption($option);
        }
        
        return $result;
    }
    
    /**
     * Reset post settings
     * 
     * @return boolean
     * 
     * @access public
     */
    public function reset() {
        AAM_Core_Cache::clear();
        
        return delete_post_meta($this->getPost()->ID, $this->getOptionName());
    }

    /**
     * Set Post. Cover all unexpectd wierd issues with WP Core
     *
     * @param WP_Post $post
     *
     * @return void
     *
     * @access public
     */
    public function setPost($post) {
        if ($post instanceof WP_Post) {
            $this->_post = $post;
        } else {
            $this->_post = (object) array('ID' => 0);
        }
    }

    /**
     * Generate option name
     * 
     * @return string
     * 
     * @access protected
     */
    protected function getOptionName() {
        $subject = $this->getSubject();
        
        //prepare option name
        $meta_key = 'aam-post-access-' . $subject->getUID();
        $meta_key .= ($subject->getId() ? $subject->getId() : '');

        return $meta_key;
    }

    /**
     * Check if option is set
     * 
     * @param string $area
     * @param string $action
     * 
     * @return boolean
     * 
     * @access public
     */
    public function has($action) {
        $option = $this->getOption();

        return (isset($option[$action]) && $option[$action]);
    }

    /**
     * Get option
     * 
     * @param string $area
     * @param string $action
     * 
     * @return boolean
     * 
     * @access public
     */
    public function get($action) {
        $option = $this->getOption();

        return (isset($option[$action]) ? $option[$action] : null);
    }
    
    /**
     * Get Post
     *
     * @return WP_Post|stdClass
     *
     * @access public
     */
    public function getPost() {
        return $this->_post;
    }
    
}