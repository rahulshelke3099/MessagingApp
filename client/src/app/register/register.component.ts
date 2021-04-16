import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
 
 
  @Output() cancelregister=new EventEmitter<boolean>();
  model:any={};
  registerForm:FormGroup;
  maxDate:Date;
  validationErrors:string[]=[];

  constructor(private accountService:AccountService,private toastr: ToastrService,
    private router:Router,private fb:FormBuilder) { }

  ngOnInit(): void {
    this.initializeForm();
    this.maxDate=new Date();
    this.maxDate.setFullYear(this.maxDate.getFullYear()-18);
    
    
  }

  initializeForm()
  {
    this.registerForm=this.fb.group({
      username:['',Validators.required],
      gender:['male',Validators.required],
      knownAs:['',Validators.required],
      dateOfBirth:['',Validators.required],
      city:['',Validators.required],
      country:['',Validators.required],
      password:['',[Validators.required,Validators.minLength(4),Validators.maxLength(8)]],
      confirmPassword: ['',[Validators.required,this.matchValue("password")]] });
 
    this.registerForm.controls.password.valueChanges.subscribe(()=>{
      this.registerForm.controls.confirmPassword.updateValueAndValidity();
    })

  }
  

  matchValue(matchTo:string):ValidatorFn
  {
    return (control:AbstractControl)=>{
  return control?.value===control?.parent?.controls[matchTo].value?null:{isMatching:true}
    }
  }

  register()
  {
   
  this.accountService.register(this.registerForm.value).subscribe(response=>{
   this.router.navigateByUrl('/members')
    this.cancel();
  },error=>{
    this.validationErrors=error;
  })
  }

  cancel()
  {
  this.cancelregister.emit(false);
  }


  // template driven
  // register()
  // {
   
  // this.accountService.register(this.model).subscribe(response=>{
  //   console.log(response);
  //   this.cancel();
  // },error=>{
  //   console.log(error);
  //   this.toastr.error(error.error)
  // })
  // }
}
