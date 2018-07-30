//
//  Test.m
//  Test
//
//  Created by TangZhuo on 2018/7/3.
//  Copyright © 2018年 TangZhuo. All rights reserved.
//

extern "C" {
    
    void _haptic(){
        UIImpactFeedbackGenerator *gen = [[UIImpactFeedbackGenerator alloc]initWithStyle:UIImpactFeedbackStyleMedium];
        [gen prepare];
        [gen impactOccurred];
    }
    
}
