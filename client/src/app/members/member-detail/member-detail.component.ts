import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

import {NgxGalleryOptions} from '@kolkov/ngx-gallery';
import {NgxGalleryImage} from '@kolkov/ngx-gallery';
import {NgxGalleryAnimation} from '@kolkov/ngx-gallery';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
 
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  member:Member;

  constructor(private memberService:MembersService,private route:ActivatedRoute) { }

  ngOnInit(): void {
    this.loadMember();
    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        thumbnailsColumns: 4,
        imagePercent: 100,
        imageAnimation: NgxGalleryAnimation.Slide
      }];
     
     
    
  }


  getImages():NgxGalleryImage[]
  {
    const imageUrls=[];
    for(const photo of this.member.photos)
    {
      imageUrls.push({
        small:photo?.url,
        medium:photo?.url,
        big:photo?.url,
      })
    }
    return imageUrls;
  }
  
  loadMember()
  {
    this.memberService.getMember(this.route.snapshot.paramMap.get('username')).subscribe(
     member=>{this.member=member;
      this.galleryImages=this.getImages();
     }
    );
  }


}
