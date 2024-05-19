import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ResponseIdeaComponent } from './response-idea.component';

describe('ResponseIdeaComponent', () => {
  let component: ResponseIdeaComponent;
  let fixture: ComponentFixture<ResponseIdeaComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ResponseIdeaComponent]
    });
    fixture = TestBed.createComponent(ResponseIdeaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
