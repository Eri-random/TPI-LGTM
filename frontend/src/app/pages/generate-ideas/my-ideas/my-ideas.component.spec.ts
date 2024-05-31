import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MyIdeasComponent } from './my-ideas.component';

describe('MisIdeasComponent', () => {
  let component: MyIdeasComponent;
  let fixture: ComponentFixture<MyIdeasComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [MyIdeasComponent]
    });
    fixture = TestBed.createComponent(MyIdeasComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
