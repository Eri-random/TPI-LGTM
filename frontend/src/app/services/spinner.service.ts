import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SpinnerService {
  private loadingSubject = new BehaviorSubject<boolean>(false);
  private loadingSubjectIdea = new BehaviorSubject<boolean>(false);
  public loading$ = this.loadingSubject.asObservable();
  public loadingIdea$ = this.loadingSubjectIdea.asObservable();

  show() {
    this.loadingSubject.next(true);
  }

  hide() {
    this.loadingSubject.next(false);
  }

  showIdea(){
    this.loadingSubjectIdea.next(true);
  }

  hideIdea(){
    this.loadingSubjectIdea.next(false);
  }
}