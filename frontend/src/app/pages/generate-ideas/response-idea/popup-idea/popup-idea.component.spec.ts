import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatDialogRef } from '@angular/material/dialog';

import { PopupIdeaComponent } from './popup-idea.component';

describe('PopupIdeaComponent', () => {
  let component: PopupIdeaComponent;
  let fixture: ComponentFixture<PopupIdeaComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [PopupIdeaComponent]
    });
    fixture = TestBed.createComponent(PopupIdeaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
